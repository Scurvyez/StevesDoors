using System;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace StevesDoors
{
    public class Dialogue_DoorColorPicker : Window
    {
        private float _colorWheelSize;
        private float _acceptButtonWidth;
        private bool _hsvColorWheelDragging;
        private Color _selectedColor;
        private Color _oldColor;
        private Action<Color> _colorSelectedCallback;

        public override Vector2 InitialSize => new (300f, 215f);

        public Dialogue_DoorColorPicker(Color oldColor, Action<Color> colorSelectedCallback)
        {
            closeOnClickedOutside = true;
            _oldColor = oldColor;
            _selectedColor = oldColor;
            _colorSelectedCallback = colorSelectedCallback;
        }

        public override void DoWindowContents(Rect inRect)
        {
            _colorWheelSize = inRect.width / 2f;
            _acceptButtonWidth = 35f;
            float colorWheelPosX = inRect.xMin;
            float colorWheelPosY = inRect.yMin;
            Widgets.HSVColorWheel(new Rect(colorWheelPosX, colorWheelPosY, _colorWheelSize, _colorWheelSize), ref _selectedColor, ref _hsvColorWheelDragging, 1f);
            ColorReadback(inRect, _selectedColor, _oldColor);


            if (Widgets.ButtonText(new Rect(inRect.center.x - _acceptButtonWidth, inRect.yMax - _acceptButtonWidth, 70f, _acceptButtonWidth), "Accept"))
            {
                _colorSelectedCallback?.Invoke(_selectedColor);
                Close();
            }
        }

        private void ColorReadback(Rect rect, Color currentColor, Color oldColor)
        {
            string labelCur = "New Color";
            string labelOld = "Old Color";

            float colorBarWidth = Mathf.Max(100f, labelCur.GetWidthCached());
            float colorBarPosX = rect.xMax - colorBarWidth;
            float colorBarPosY = rect.yMin;

            Widgets.Label(new Rect(colorBarPosX, colorBarPosY, colorBarWidth, colorBarWidth / 5), labelCur);
            Widgets.DrawBoxSolid(new Rect(colorBarPosX, colorBarPosY + (colorBarWidth / 5f), colorBarWidth, colorBarWidth / 5f), currentColor);

            Widgets.Label(new Rect(colorBarPosX, colorBarPosY + ((colorBarWidth / 5f) * 2.5f), colorBarWidth, colorBarWidth / 5), labelOld);
            Widgets.DrawBoxSolid(new Rect(colorBarPosX, colorBarPosY + ((colorBarWidth / 5f) * 3.5f), colorBarWidth, colorBarWidth / 5f), oldColor);
        }
    }
}
