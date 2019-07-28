/// Credit David Gileadi
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/pull-requests/11

using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    // Stepper control
    [AddComponentMenu("UI/Extensions/Stepper")]
    [RequireComponent(typeof(RectTransform))]
    public class Stepper : UIBehaviour
    {
        private Selectable[] _sides;
        [SerializeField]
        [Tooltip("The current step value of the control")]
        private float _value = 0;
        [SerializeField]
        [Tooltip("The minimum step value allowed by the control. When reached it will disable the '-' button")]
        private float _minimum = 0;
        [SerializeField]
        [Tooltip("The maximum step value allowed by the control. When reached it will disable the '+' button")]
        private float _maximum = 100;
        [SerializeField]
        [Tooltip("The step increment used to increment / decrement the step value")]
        private float _step = 1;
        [SerializeField]
        [Tooltip("Does the step value loop around from end to end")]
        private bool _wrap = false;
        [SerializeField]
        [Tooltip("A GameObject with an Image to use as a separator between segments. Size of the RectTransform will determine the size of the separator used.\nNote, make sure to disable the separator GO so that it does not affect the scene")]
        private Graphic _separator;
        private float _separatorWidth = 0;

        private float separatorWidth
        {
            get
            {
                if (_separatorWidth == 0 && separator)
                {
                    _separatorWidth = separator.rectTransform.rect.width;
                    var image = separator.GetComponent<Image>();
                    if (image)
                        _separatorWidth /= image.pixelsPerUnit;
                }
                return _separatorWidth;
            }
        }

        // Event delegates triggered on click.
        [SerializeField]
        private StepperValueChangedEvent _onValueChanged = new StepperValueChangedEvent();

        [Serializable]
        public class StepperValueChangedEvent : UnityEvent<float> { }

        public Selectable[] sides
        {
            get
            {
                if (_sides == null || _sides.Length == 0)
                {
                    _sides = GetSides();
                }
                return _sides;
            }
        }

        public float value { get { return _value; } set { _value = value; } }

        public float minimum { get { return _minimum; } set { _minimum = value; } }

        public float maximum { get { return _maximum; } set { _maximum = value; } }

        public float step { get { return _step; } set { _step = value; } }

        public bool wrap { get { return _wrap; } set { _wrap = value; } }

        public Graphic separator { get { return _separator; } set { _separator = value; _separatorWidth = 0; LayoutSides(sides); } }

        public StepperValueChangedEvent onValueChanged
        {
            get { return _onValueChanged; }
            set { _onValueChanged = value; }
        }

        protected Stepper()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (separator)
                LayoutSides();

            if (!wrap)
            {
                DisableAtExtremes(sides);
            }
        }
#endif

        private Selectable[] GetSides()
        {
            var buttons = GetComponentsInChildren<Selectable>();
            if (buttons.Length != 2)
            {
                throw new InvalidOperationException("A stepper must have two Button children");
            }

            for (int i = 0; i < 2; i++)
            {
                var side = buttons[i].GetComponent<StepperSide>();
                if (side == null)
                {
                    side = buttons[i].gameObject.AddComponent<StepperSide>();
                }
            }

            if (!wrap)
            {
                DisableAtExtremes(buttons);
            }
            LayoutSides(buttons);

            return buttons;
        }

        public void StepUp()
        {   
            float v = GeneticUIController.Instance.PriceValue(this).getPrice;

            if (CurrencyController.Instance.RemoveCurrency(Mathf.RoundToInt(v),true)==true) {
                GeneticUIController.Instance.PriceValue(this).Inc();
                Step(step);
                GeneticUIController.Instance.CarryOut(GeneticUIController.Instance.PriceValue(this).valueName,GeneticUIController.Instance.PriceValue(this).stepper.step,GeneticUIController.Instance.PriceValue(this));
            }
        }

        public void StepDown()
        {
            float v = GeneticUIController.Instance.PriceValue(this).getPrice;

            if (CurrencyController.Instance.RemoveCurrency(Mathf.RoundToInt(v),true)==true) {
                GeneticUIController.Instance.PriceValue(this).Inc();
                Step(-step);
                GeneticUIController.Instance.CarryOut(GeneticUIController.Instance.PriceValue(this).valueName,-GeneticUIController.Instance.PriceValue(this).stepper.step,GeneticUIController.Instance.PriceValue(this));
            }
        }

        private void Step(float amount)
        {
            value += amount;

            if (wrap)
            {
                if (value > maximum) value = minimum;
                if (value < minimum) value = maximum;
            }
            else
            {
                value = Math.Max(minimum, value);
                value = Math.Min(maximum, value);

                DisableAtExtremes(sides);
            }

            _onValueChanged.Invoke(value);
        }

        private void DisableAtExtremes(Selectable[] sides)
        {
            sides[0].interactable = wrap || value > minimum;
            sides[1].interactable = wrap || value < maximum;
        }

        private void RecreateSprites(Selectable[] sides)
        {
            for (int i = 0; i < 2; i++)
            {
                if (sides[i].image == null)
                    continue;

                var sprite = sides[i].image.sprite;
                if (sprite.border.x == 0 || sprite.border.z == 0)
                    continue;

                var rect = sprite.rect;
                var border = sprite.border;

                if (i == 0)
                {
                    rect.xMax = border.z;
                    border.z = 0;
                }
                else
                {
                    rect.xMin = border.x;
                    border.x = 0;
                }

                sides[i].image.sprite = Sprite.Create(sprite.texture, rect, sprite.pivot, sprite.pixelsPerUnit, 0, SpriteMeshType.FullRect, border);
            }
        }

        public void LayoutSides(Selectable[] sides = null)
        {
            sides = sides ?? this.sides;

            RecreateSprites(sides);

            RectTransform transform = this.transform as RectTransform;
            float width = (transform.rect.width / 2) - separatorWidth;

            for (int i = 0; i < 2; i++)
            {
                float insetX = i == 0 ? 0 : width + separatorWidth;

                var rectTransform = sides[i].GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, insetX, width);
                rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, transform.rect.height);

            // TODO: maybe adjust text position
            }

            if (separator)
            {
                var sepTransform = gameObject.transform.Find("Separator");
                Graphic sep = (sepTransform != null) ? sepTransform.GetComponent<Graphic>() : (GameObject.Instantiate(separator.gameObject) as GameObject).GetComponent<Graphic>();
                sep.gameObject.name = "Separator";
                sep.gameObject.SetActive(true);
                sep.rectTransform.SetParent(this.transform, false);
                sep.rectTransform.anchorMin = Vector2.zero;
                sep.rectTransform.anchorMax = Vector2.zero;
                sep.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, width, separatorWidth);
                sep.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, transform.rect.height);
            }
        }
    }

    [RequireComponent(typeof(Selectable))]
    public class StepperSide : UIBehaviour, IPointerClickHandler, ISubmitHandler
    {
        Selectable button { get { return GetComponent<Selectable>(); } }

        Stepper stepper { get { return GetComponentInParent<Stepper>(); } }

        bool leftmost { get { return button == stepper.sides[0]; } }

        protected StepperSide()
        { }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();
        }

        private void Press()
        {
            if (!button.IsActive() || !button.IsInteractable())
                return;

            if (leftmost)
            {
                stepper.StepDown();
            }
            else
            {
                stepper.StepUp();
            }
        }
    }
}