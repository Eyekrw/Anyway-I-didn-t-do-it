using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SourGrape.kiyoung
{
    using UnityEngine;
    using UnityEngine.UI;

    public class GuideModal : MonoBehaviour
    {
        public GameObject guideModal;  // 모달 전체 패널
        public Button helpButton;      // ? 버튼
        public Button closeButton;     // 모달 닫기 버튼

        void Start()
        {
            // 초기 상태에서 모달 비활성화
            guideModal.SetActive(false);

            // 버튼 클릭 이벤트 등록
            helpButton.onClick.AddListener(OpenGuideModal);
            closeButton.onClick.AddListener(CloseGuideModal);
        }

        // 모달 창 열기
        void OpenGuideModal()
        {
            guideModal.SetActive(true);
        }

        // 모달 창 닫기
        void CloseGuideModal()
        {
            guideModal.SetActive(false);
        }
    }
}
