using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

namespace SourGrape.bbbb
{
    public class ErrorPanel : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public CanvasGroup Target;

        public string message = "";

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(LifeCycle());
        }

        private void Update()
        {
            text.text = message;
        }

        IEnumerator LifeCycle()
        {
            yield return new WaitForSeconds(2);
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            float elapsedTime = 0.0f;

            // Alpha 값을 1에서 0으로 서서히 변경
            while (elapsedTime < 1)
            {
                elapsedTime += Time.deltaTime;
                Target.alpha = Mathf.Clamp01(1.0f - (elapsedTime / 1));
                yield return null;
            }

            // CanvasGroup 비활성화
            Target.interactable = false;
            Target.blocksRaycasts = false;
            Destroy(this.gameObject);
        }

    }
}
