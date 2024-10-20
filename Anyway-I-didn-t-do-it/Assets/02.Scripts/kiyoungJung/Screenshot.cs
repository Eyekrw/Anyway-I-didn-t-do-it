using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SourGrape.kiyoung
{
    public class Screenshot : MonoBehaviour
    {
        private int screenshotCount = 0;

        void Update()
        {
            // "P" 키를 눌렀을 때 스크린샷을 찍고 저장
            if (Input.GetKeyDown(KeyCode.P))
            {
                // 스크린샷을 프로젝트 폴더의 `Screenshots` 폴더에 저장
                string filename = $"Screenshots/screenshot_{screenshotCount}.png";
                ScreenCapture.CaptureScreenshot(filename);
                screenshotCount++;
                Debug.Log($"Screenshot saved at: {filename}");
            }
        }
    }
}
