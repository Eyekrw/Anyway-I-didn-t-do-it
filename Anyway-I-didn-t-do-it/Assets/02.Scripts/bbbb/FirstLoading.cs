using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SourGrape.bbbb
{
    public class FirstLoading : MonoBehaviour
    {
        public GameObject mainCanvas;
        public Image targetImage;
        
        float fadeDuration = 2.0f;
        

        // Start is called before the first frame update
        void Start()
        {
            mainCanvas.SetActive(false);
            StartCoroutine(FadeIn());
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        IEnumerator FadeIn()
        {
            // 알파 값을 0에서 1까지 서서히 증가
            float elapsedTime = 0.0f;
            Color imageColor = targetImage.color;
            imageColor.a = 0;
            targetImage.color = imageColor;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                imageColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                targetImage.color = imageColor;
                yield return null;
            }
            StartCoroutine(FadeOut());
        }

        // 페이드 아웃 (서서히 사라지게 함)
        IEnumerator FadeOut()
        {
            // 알파 값을 1에서 0까지 서서히 감소
            float elapsedTime = 0.0f;
            Color imageColor = targetImage.color;
            imageColor.a = 1;
            targetImage.color = imageColor;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                imageColor.a = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
                targetImage.color = imageColor;
                yield return null;
            }
            yield return new WaitForSeconds(0.4f);
            mainCanvas.SetActive(true);
            this.gameObject.SetActive(false);
            Destroy(this);
        }
    }
}
