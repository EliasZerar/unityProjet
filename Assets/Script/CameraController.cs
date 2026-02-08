using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera m_PlayerCamera;

    private CinemachineCamera m_ActiveInteractionCamera;

    private const int PRIORITY_HIGH = 10;
    private const int PRIORITY_LOW  = 0;

    private void Awake()
    {
        ActivateCamera(m_PlayerCamera);
    }

    public void SwitchToInteractionCamera(CinemachineCamera interactionCamera)
    {
        m_ActiveInteractionCamera = interactionCamera;
        DeactivateCamera(m_PlayerCamera);
        ActivateCamera(m_ActiveInteractionCamera);
    }

    public void SwitchBackToPlayerCamera()
    {
        if (m_ActiveInteractionCamera != null)
            DeactivateCamera(m_ActiveInteractionCamera);

        ActivateCamera(m_PlayerCamera);
        m_ActiveInteractionCamera = null;
    }

    private void ActivateCamera(CinemachineCamera camera)
    {
        if (camera != null)
            camera.Priority = PRIORITY_HIGH;
    }

    private void DeactivateCamera(CinemachineCamera camera)
    {
        if (camera != null)
            camera.Priority = PRIORITY_LOW;
    }
}