using UnityEngine;
using TMPro;

public class ElevatorManager : MonoBehaviour
{
    [Header("--- UI Setup ---")]
    [Tooltip("화면에 띄울 층 선택 UI 패널 (Elevator_UI)")]
    public GameObject elevatorUIPanel;

    [Header("--- Display Setup ---")]
    [Tooltip("엘리베이터 위 중앙에 붙인 층수 표시 텍스트 (Floor_Text)")]
    public TextMeshProUGUI floorDisplayText;

    [Header("--- Floor Coordinates Setup ---")]
    [Tooltip("각 층의 순간이동 목표 좌표들 (0번방=1층, 1번방=2층, 2번방=3층...)")]
    public Vector3[] floorCoordinates = new Vector3[]
    {
        new Vector3(0f, 1f, 0f),     // 1층 좌표
        new Vector3(0f, 11.5f, 0f),  // 2층 좌표
        new Vector3(0f, 21.5f, 0f),  // 3층 좌표
        new Vector3(0f, 31.5f, 0f),  // 4층 좌표
        new Vector3(0f, 41.5f, 0f),  // 5층 좌표
        new Vector3(0f, 51.5f, 0f)   // 6층 좌표 (바닥 높이에서 약 1.5m 여유 준 높이)
    };

    [Header("--- Range & Delay Setup ---")]
    [Tooltip("엘리베이터 중심으로부터 UI가 켜지는 반지름 거리")]
    public float detectionRadius = 5.0f;
    [Tooltip("영역에 진입한 후 UI가 뜰 때까지의 대기 시간 (초 단위)")]
    public float uiOpenDelay = 1.5f;

    private GameObject playerObject;
    private bool isPlayerInside = false;
    private bool isUIOpen = false;

    void Start()
    {
        if (elevatorUIPanel != null)
            elevatorUIPanel.SetActive(false);

        playerObject = GameObject.FindGameObjectWithTag("Player");

        // 시작할 때 내 현재 높이를 기준으로 전광판 초기화
        if (playerObject != null)
        {
            UpdateFloorDisplay(transform.position.y);
        }
    }

    void Update()
    {
        if (playerObject == null) return;

        // 플레이어와의 거리 계산
        float distance = Vector3.Distance(transform.position, playerObject.transform.position);

        if (distance <= detectionRadius)
        {
            if (!isPlayerInside)
            {
                isPlayerInside = true;
                if (!isUIOpen)
                {
                    Invoke("OpenUI", uiOpenDelay);
                }
            }
            // 범위 안에 서 있는 동안 실시간으로 층수 전광판 갱신
            UpdateFloorDisplay(playerObject.transform.position.y);
        }
        else
        {
            if (isPlayerInside)
            {
                isPlayerInside = false;
                CancelInvoke("OpenUI");
                if (isUIOpen) CloseUI();
            }
        }
    }

    private void OpenUI()
    {
        if (elevatorUIPanel != null && isPlayerInside)
        {
            elevatorUIPanel.SetActive(true);
            isUIOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void CloseUI()
    {
        if (elevatorUIPanel != null)
        {
            elevatorUIPanel.SetActive(false);
            isUIOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// UI의 각 층 버튼들과 연결할 범용 이동 함수 (인스펙터 버튼 OnClick에서 사용)
    /// </summary>
    /// <param name="floorIndex">0=1층, 1=2층, 2=3층 ... 5=6층</param>
    public void MoveToFloor(int floorIndex)
    {
        // 배열 범위를 벗어나지 않았는지 안전성 검사(예외 처리)
        if (floorIndex < 0 || floorIndex >= floorCoordinates.Length)
        {
            Debug.LogError($"존재하지 않는 층수 인덱스입니다: {floorIndex}");
            return;
        }

        if (playerObject != null)
        {
            CharacterController cc = playerObject.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // 해당 층의 좌표로 안전하게 텔레포트
            playerObject.transform.position = floorCoordinates[floorIndex];

            if (cc != null) cc.enabled = true;
        }

        CancelInvoke("OpenUI");
        isPlayerInside = false;
        CloseUI();
    }

    /// <summary>
    /// Y축 높이에 따라 자동으로 몇 층인지 계산하여 전광판 글자를 바꾸는 함수
    /// </summary>
    private void UpdateFloorDisplay(float playerY)
    {
        if (floorDisplayText == null) return;

        // Y축 높이를 기준으로 직관적으로 층수 계산 (대략 10m 단위 복층 구조 기준)
        if (playerY >= 50f) floorDisplayText.text = "6F";
        else if (playerY >= 40f) floorDisplayText.text = "5F";
        else if (playerY >= 30f) floorDisplayText.text = "4F";
        else if (playerY >= 20f) floorDisplayText.text = "3F";
        else if (playerY >= 10f) floorDisplayText.text = "2F";
        else floorDisplayText.text = "1F";
    }
}