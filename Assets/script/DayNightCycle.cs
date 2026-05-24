using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;
    public float rotationSpeed = 10f;

    void Update()
    {
        sun.transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);

        float intensity = Mathf.Clamp01(-sun.transform.forward.y);
        sun.intensity = Mathf.Lerp(0.2f, 1f, intensity);
    }
}