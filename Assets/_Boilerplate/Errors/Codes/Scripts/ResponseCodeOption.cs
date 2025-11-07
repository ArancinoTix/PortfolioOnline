using UnityEngine;
using UnityEngine.Localization;

namespace U9.Errors.Codes
{
    [CreateAssetMenu(fileName = "ResponseCodeOption", menuName = "UNIT9/Errors/Response Code Option")]
    public class ResponseCodeOption : ScriptableObject
    {
        [SerializeField] private int _code;
        [SerializeField] private LocalizedString _title;
        [SerializeField] private LocalizedString _message;
        [SerializeField] private Sprite _icon;
        [SerializeField] private LocalizedString[] _customChoices;
        [SerializeField] private bool _showDefaultCloseChoice = true;

        public Sprite Icon { get => _icon; }
        public LocalizedString Message { get => _message; }
        public LocalizedString Title { get => _title; }
        public int Code { get => _code; }
        public LocalizedString[] CustomChoices { get => _customChoices;}
        public bool ShowDefaultCloseChoice { get => _showDefaultCloseChoice || _customChoices.Length ==0; }
    }
}
