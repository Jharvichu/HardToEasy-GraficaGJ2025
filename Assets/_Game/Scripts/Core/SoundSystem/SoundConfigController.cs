using UnityEngine;
using UnityEngine.UI;

public class SoundConfigController : MonoBehaviour {

	[SerializeField] Slider _generalSlider;

	void Start() {
		if(_generalSlider != null) {
			_generalSlider.value = AudioManager.Instance.GetGeneralVolume();
            _generalSlider.onValueChanged.AddListener(UpdateGeneralVolume);
		}
	}

	public void UpdateGeneralVolume(float value){
		AudioManager.Instance.UpdateGeneralVolume(value);
	}
}