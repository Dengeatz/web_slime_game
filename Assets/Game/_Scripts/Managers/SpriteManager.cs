using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask.Core;
using UnityEngine;

namespace TestTask.Assets.Game._Scripts.Managers
{
    public class SpriteManager : MonoBehaviour
    {
        [SerializeField] public Sprite RedFullySprite;
        [SerializeField] public Sprite GreenFullySprite;
        [SerializeField] public Sprite YellowFullySprite;
        [SerializeField] public Sprite BlueFullySprite;
        [SerializeField] public Sprite RedShakeSprite;
        [SerializeField] public Sprite GreenShakeSprite;
        [SerializeField] public Sprite YellowShakeSprite;
        [SerializeField] public Sprite BlueShakeSprite;

        #region Public Methods
        public Sprite GetBulbFullySprite(Slime slime)
        {
            switch(slime.SlimeColorType)
            {
                case Enums.SlimeColor.Red:
                    return RedFullySprite;
                case Enums.SlimeColor.Green:
                    return GreenFullySprite;
                case Enums.SlimeColor.Yellow:
                    return YellowFullySprite;
                case Enums.SlimeColor.Blue:
                    return BlueFullySprite;
                default:
                    return null;
            }
        }

        public Sprite GetBulbShakeSprite(Slime slime)
        {
            switch (slime.SlimeColorType)
            {
                case Enums.SlimeColor.Red:
                    return RedShakeSprite;
                case Enums.SlimeColor.Green:
                    return GreenShakeSprite;
                case Enums.SlimeColor.Yellow:
                    return YellowShakeSprite;
                case Enums.SlimeColor.Blue:
                    return BlueShakeSprite;
                default:
                    return null;
            }
        }
        #endregion
    }
}
