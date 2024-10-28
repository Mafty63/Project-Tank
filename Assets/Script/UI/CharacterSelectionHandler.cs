using System.Collections;
using System.Collections.Generic;
using ProjectTank.Utilities;
using UnityEngine;

namespace ProjectTank
{
    public class CharacterSelectionHandler : SingletonMonoBehaviour<CharacterSelectionHandler>
    {
        [SerializeField] private CharacterSelectPlayer characterPlayer;
        [SerializeField] private List<CharacterSelectButton> characterSelectButtons;
        [SerializeField] private CharacterSelectButton defaultCharacter;
        private CharacterSelectButton currentSelectedCharacter;

        private void Start()
        {
            ChangeCharacter(defaultCharacter);
        }

        public void ChangeCharacter(CharacterSelectButton selectedCharacter)
        {
            if (currentSelectedCharacter != null)
                currentSelectedCharacter.Selected(false);

            currentSelectedCharacter = selectedCharacter;
            currentSelectedCharacter.Selected(true);

            characterPlayer.UpdateCharacter(currentSelectedCharacter.CharacterId);
            GameMultiplayer.Instance.ChangePlayerCharacter(selectedCharacter.CharacterId);
        }
    }
}
