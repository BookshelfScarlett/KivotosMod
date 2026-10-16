using System;
using KivotosMod.Runtime.Spine38;

namespace KivotosMod.Content.Tablet;

// Separate from the GPU renderer so the real skeleton/animation can be checked offline.
internal sealed class TabletSpine38Animation
{
    internal const float MinBlinkInterval = 3f;
    internal const float MaxBlinkInterval = 6f;
    internal const float TrackingResponseSeconds = 0.18f;
    internal const float MaxTrackingSpeed = 4f;
    private const float BlinkFade = 0.08f;
    private readonly AnimationState _state;
    private readonly Animation _blink;
    private readonly Random _random;
    private readonly Slot _pupils;
    private readonly float[] _pupilVertices;
    private readonly Bone _headControl;
    private readonly float _turnMinX, _turnMaxX, _turnMinY, _turnMaxY;
    private float _mouseX, _mouseY, _lookX, _lookY;
    private float _focusX, _focusY;
    private bool _hasMouse;
    private float _blinkTimer;
    private float _blinkRemaining;
    public Skeleton Skeleton { get; }
    public float BlinkDuration => _blink.Duration;
    public bool Blinking => _blinkRemaining > 0;

    public TabletSpine38Animation(SkeletonData data, Random random = null, bool sora = false)
    {
        Skeleton = new Skeleton(data);
        _random = random ?? Random.Shared;
        _blink = CreateBlink(data, sora);
        if (!sora)
        {
            _headControl = Skeleton.FindBone("bone18") ?? throw new InvalidOperationException("Arona head control 'bone18' is missing.");
            Animation turning = data.FindAnimation("Turning") ?? throw new InvalidOperationException("Arona head animation 'Turning' is missing.");
            TranslateTimeline control = null;
            foreach (Timeline timeline in turning.Timelines)
                if (timeline is TranslateTimeline translate && timeline is not ScaleTimeline
                    && translate.BoneIndex == _headControl.Data.Index) { control = translate; break; }
            if (control == null) throw new InvalidOperationException("Arona head control range is missing.");
            for (int i = 0; i < control.Frames.Length; i += 3)
            {
                _turnMinX = Math.Min(_turnMinX, control.Frames[i + 1]);
                _turnMaxX = Math.Max(_turnMaxX, control.Frames[i + 1]);
                _turnMinY = Math.Min(_turnMinY, control.Frames[i + 2]);
                _turnMaxY = Math.Max(_turnMaxY, control.Frames[i + 2]);
            }
        }
        foreach (Slot slot in Skeleton.Slots)
            if (slot.Attachment is MeshAttachment mesh && mesh.Name == "Hitomi")
            {
                _pupils = slot;
                _pupilVertices = new float[mesh.WorldVerticesLength];
                break;
            }
        if (_pupils == null) throw new InvalidOperationException("Portrait pupil mesh 'Hitomi' is missing.");
        _state = new AnimationState(new AnimationStateData(data));
        _state.SetAnimation(0, data.FindAnimation(sora ? "animation" : "swing") ?? throw new InvalidOperationException("Portrait idle animation is missing."), true);
        _blinkTimer = NextInterval();
        Update(0);
    }

    public void Update(float seconds)
    {
        seconds = Math.Clamp(seconds, 0, 0.1f);
        if (_blinkRemaining > 0) _blinkRemaining = Math.Max(0, _blinkRemaining - seconds);
        else if ((_blinkTimer -= seconds) <= 0) Blink();
        _state.Update(seconds);
        _state.Apply(Skeleton);
        // Sample the unturned face as a stable reference. Restore the control
        // before each solve so the tracking offset cannot accumulate or feed back.
        if (_headControl != null) { _headControl.X = _headControl.Data.X; _headControl.Y = _headControl.Data.Y; }
        Skeleton.UpdateWorldTransform();
        var pupils = (MeshAttachment)_pupils.Attachment;
        pupils.ComputeWorldVertices(_pupils, 0, _pupilVertices.Length, _pupilVertices, 0, 2);
        float x = 0, y = 0;
        for (int i = 0; i < _pupilVertices.Length; i += 2) { x += _pupilVertices[i]; y += _pupilVertices[i + 1]; }
        x /= _pupilVertices.Length / 2; y /= _pupilVertices.Length / 2;
        float targetX = _hasMouse ? Math.Clamp((_mouseX - x) / 100f, -1, 1) : 0;
        float targetY = _hasMouse ? Math.Clamp((_mouseY - y) / 100f, -1, 1) : 0;
        float followBlend = 1 - MathF.Exp(-seconds / TrackingResponseSeconds);
        float maxStep = MaxTrackingSpeed * seconds;
        if (_headControl == null) { _focusX = targetX; _focusY = targetY; }
        else if (maxStep > 0)
        {
            // A smooth speed limit damps alternating targets without the residual
            // bias produced by clipping equal positive/negative steps abruptly.
            _focusX += maxStep * MathF.Tanh((targetX - _focusX) * followBlend / maxStep);
            _focusY += maxStep * MathF.Tanh((targetY - _focusY) * followBlend / maxStep);
        }
        if (_headControl != null)
        {
            // The authored negative "change" constraint moves face details
            // opposite bone18. Invert both axes, within the Turning key ranges.
            float turnX = _focusX >= 0 ? _focusX * _turnMinX : -_focusX * _turnMaxX;
            float turnY = _focusY >= 0 ? _focusY * _turnMinY : -_focusY * _turnMaxY;
            _headControl.X = _headControl.Data.X + turnX;
            _headControl.Y = _headControl.Data.Y + turnY;
            Skeleton.UpdateWorldTransform();
        }
        // Head tracking continues through blinking; hold the pupil gaze until
        // reopening, then smoothly catch up to the same filtered focus point.
        if (Blinking) return;
        float blend = 1 - MathF.Exp(-12f * seconds);
        _lookX += (_focusX * 2.5f - _lookX) * blend;
        _lookY += (_focusY * 1.4f - _lookY) * blend;
    }

    // Coordinates are skeleton world coordinates, before the UI renderer's Y flip.
    public void LookAt(float x, float y) { _mouseX = x; _mouseY = y; _hasMouse = true; }

    // Pupil weights also affect the whites/lids. Offset this attachment only, after
    // computing its animated vertices, so the rig and source data stay untouched.
    public void ApplyEyeTracking(Attachment attachment, float[] vertices, int count)
    {
        if (attachment != _pupils.Attachment) return;
        for (int i = 0; i < count; i += 2) { vertices[i] += _lookX; vertices[i + 1] += _lookY; }
    }

    // Gaze tracking must never fade the pupil. Keep the API used by the offline
    // pose probe; the shader still applies the source slot/skeleton alpha as usual.
    public float EyeOpacity(Attachment attachment) => 1f;

    public void Blink()
    {
        if (Blinking) return;
        _state.SetAnimation(1, _blink, false);
        _state.AddEmptyAnimation(1, BlinkFade, 0);
        _blinkRemaining = _blink.Duration + BlinkFade;
        _blinkTimer = NextInterval();
    }

    private float NextInterval() => MinBlinkInterval + (float)_random.NextDouble() * (MaxBlinkInterval - MinBlinkInterval);

    internal static bool IsEyeBone(string name, bool sora) => sora
        ? name.StartsWith("eye", StringComparison.Ordinal) || name == "face3" || name == "face4"
        : name.StartsWith("eyes", StringComparison.Ordinal);

    private static Animation CreateBlink(SkeletonData data, bool sora)
    {
        Animation original = data.FindAnimation("blink") ?? throw new InvalidOperationException("Portrait animation 'blink' is missing.");
        float start = float.PositiveInfinity, end = 0;
        // Find just the changed eye keys and their surrounding setup keys. Sora's
        // export includes static keys at 0/end, outside its actual 1.167–1.6 blink.
        foreach (Timeline timeline in original.Timelines)
        {
            if (timeline is not IBoneTimeline bone || !IsEyeBone(data.Bones.Items[bone.BoneIndex].Name, sora)) continue;
            float[] frames = timeline is RotateTimeline r ? r.Frames : ((TranslateTimeline)timeline).Frames;
            int stride = timeline is RotateTimeline ? 2 : 3;
            float setup = timeline is ScaleTimeline ? 1 : 0;
            for (int i = 0; i < frames.Length; i += stride)
            {
                bool changed = false;
                for (int j = 1; j < stride; j++) changed |= Math.Abs(frames[i + j] - setup) > 0.0001f;
                if (!changed) continue;
                start = Math.Min(start, frames[Math.Max(0, i - stride)]);
                end = Math.Max(end, frames[Math.Min(frames.Length - stride, i + stride)]);
            }
        }
        if (end <= start) throw new InvalidOperationException("Portrait blink has no eye keyframes.");
        var timelines = new ExposedList<Timeline>();
        foreach (Timeline timeline in original.Timelines)
        {
            if (timeline is not IBoneTimeline bone || !IsEyeBone(data.Bones.Items[bone.BoneIndex].Name, sora)) continue;
            float[] source = timeline is RotateTimeline r ? r.Frames : ((TranslateTimeline)timeline).Frames;
            int stride = timeline is RotateTimeline ? 2 : 3;
            int first = 0, last = source.Length - stride;
            while (first < last && source[first] < start - 0.0001f) first += stride;
            while (last > first && source[last] > end + 0.0001f) last -= stride;
            int count = (last - first) / stride + 1;
            float[] frames = new float[count * stride];
            Array.Copy(source, first, frames, 0, frames.Length);
            for (int i = 0; i < frames.Length; i += stride) frames[i] -= start;
            CurveTimeline copy;
            if (timeline is RotateTimeline)
                copy = new RotateTimeline(count) { BoneIndex = bone.BoneIndex, Frames = frames };
            else
            {
                TranslateTimeline clone = timeline is ScaleTimeline ? new ScaleTimeline(count) : new TranslateTimeline(count);
                clone.BoneIndex = bone.BoneIndex; clone.Frames = frames; copy = clone;
            }
            // Keep the runtime's original sampled Bezier segments for retained keys.
            if (count > 1) Array.Copy(((CurveTimeline)timeline).curves, first / stride * 19, copy.curves, 0, copy.curves.Length);
            timelines.Add(copy);
        }
        return new Animation("tablet_blink", timelines, end - start);
    }
}
