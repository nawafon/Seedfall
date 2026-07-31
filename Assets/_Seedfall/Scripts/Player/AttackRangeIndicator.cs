using UnityEngine;

namespace Seedfall.Player
{
    // Ground-level combat range visualization, always active during Play. The outer ring
    // (radius = BareHandMelee.AttackRange) shows max possible reach in any direction, but
    // on its own doesn't show WHICH direction actually gets hit -- the real hit check is a
    // sphere centered on a single point in front of the player, not the whole ring. The
    // facing line + hit-zone circle fix that: both are children positioned with a local Z
    // offset, so Unity's normal transform hierarchy applies the player's current rotation
    // to them every frame automatically -- no per-frame recompute needed, and they always
    // point at exactly the spot BareHandMelee.TryAttack's
    // "transform.position + transform.forward * attackRange" checks.
    [RequireComponent(typeof(BareHandMelee))]
    [RequireComponent(typeof(CharacterController))]
    public class AttackRangeIndicator : MonoBehaviour
    {
        [SerializeField] private int rangeRingSegments = 48;
        [SerializeField] private int hitZoneSegments = 24;
        [SerializeField] private float lineWidth = 0.08f;
        [SerializeField] private float groundClearance = 0.03f; // avoids z-fighting with the ground plane
        [SerializeField] private Color rangeRingColor = Color.white;              // max reach, any direction
        [SerializeField] private Color hitZoneColor = new Color(1f, 0.3f, 0.15f); // exact spot a swing lands
        [SerializeField] private Color facingLineColor = Color.yellow;            // connects the two so facing is unambiguous

        private BareHandMelee _melee;
        private float _feetLocalY;

        private LineRenderer _rangeRing;
        private Transform _hitZonePivot;
        private LineRenderer _hitZoneCircle;
        private LineRenderer _facingLine;

        private float _lastRange = -1f;
        private float _lastRadius = -1f;

        private void Awake()
        {
            _melee = GetComponent<BareHandMelee>();
            CharacterController controller = GetComponent<CharacterController>();
            _feetLocalY = controller.center.y - controller.height * 0.5f + groundClearance;

            _rangeRing = CreateRing("AttackRangeRing", transform, rangeRingSegments, rangeRingColor);
            _rangeRing.transform.localPosition = new Vector3(0f, _feetLocalY, 0f);

            _hitZonePivot = new GameObject("AttackHitZonePivot").transform;
            _hitZonePivot.SetParent(transform, worldPositionStays: false);
            _hitZoneCircle = CreateRing("AttackHitZoneCircle", _hitZonePivot, hitZoneSegments, hitZoneColor);

            GameObject lineObject = new GameObject("AttackFacingLine");
            lineObject.transform.SetParent(transform, worldPositionStays: false);
            lineObject.transform.localPosition = new Vector3(0f, _feetLocalY, 0f);
            _facingLine = lineObject.AddComponent<LineRenderer>();
            ConfigureLine(_facingLine, facingLineColor);
            _facingLine.loop = false;
            _facingLine.positionCount = 2;

            RedrawAll();
        }

        private void Update()
        {
            float range = _melee.AttackRange;
            float radius = _melee.AttackRadius;
            if (Mathf.Approximately(range, _lastRange) && Mathf.Approximately(radius, _lastRadius))
            {
                return;
            }
            _lastRange = range;
            _lastRadius = radius;
            RedrawAll();
        }

        private void RedrawAll()
        {
            float range = _melee.AttackRange;
            float radius = _melee.AttackRadius;

            DrawCircle(_rangeRing, range);
            DrawCircle(_hitZoneCircle, radius);

            // Local Z offset only -- Unity re-applies the player's current rotation to this
            // child every frame on its own, so the hit zone tracks facing for free.
            _hitZonePivot.localPosition = new Vector3(0f, _feetLocalY, range);

            _facingLine.SetPosition(0, Vector3.zero);
            _facingLine.SetPosition(1, new Vector3(0f, 0f, range));
        }

        private LineRenderer CreateRing(string name, Transform parent, int segments, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, worldPositionStays: false);
            LineRenderer line = go.AddComponent<LineRenderer>();
            ConfigureLine(line, color);
            line.loop = true;
            line.positionCount = segments;
            return line;
        }

        private void ConfigureLine(LineRenderer line, Color color)
        {
            line.useWorldSpace = false;
            line.widthMultiplier = lineWidth;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }
            Material mat = new Material(shader);
            // Mirrors the _BaseColor/_Color check already used by BareHandMelee.FlashRoutine
            // and PlantPlot.TintRenderer so this works under whichever shader resolved above.
            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", color);
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", color);
            }
            line.material = mat;
        }

        private void DrawCircle(LineRenderer line, float radius)
        {
            int segments = line.positionCount;
            for (int i = 0; i < segments; i++)
            {
                float angle = 2f * Mathf.PI * i / segments;
                Vector3 point = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                line.SetPosition(i, point);
            }
        }
    }
}
