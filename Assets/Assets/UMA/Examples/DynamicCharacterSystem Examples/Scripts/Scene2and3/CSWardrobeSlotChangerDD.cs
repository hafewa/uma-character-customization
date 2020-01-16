using UnityEngine;
using UnityEngine.UI;

namespace UMA.CharacterSystem.Examples
{
	public class CSWardrobeSlotChangerDD : MonoBehaviour
	{
		public string wardrobeSlotToChange;

		public TestCustomizerDD customizerScript;

		public void ChangeWardrobeSlot(int slotId)
        {
            Debug.Log(wardrobeSlotToChange + " " + (slotId - 1));
			customizerScript.SetSlot(wardrobeSlotToChange, slotId -1);
		}
	}
}
