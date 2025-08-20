using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.Mathematics;

public class CardBarAnimations : MonoBehaviour
{
    [Header("Health Bar Components")]
    [SerializeField] private Image PVBar;
    [SerializeField] private Image PVBarBackground;
    [SerializeField] private TMP_Text maxPVTextBar;
    [SerializeField] private TMP_Text currentPVTextBar;

    [Header("Barrier Bar Components")]
    [SerializeField] private Image PEBar;
    [SerializeField] private TMP_Text maxPETextBar;
    [SerializeField] private TMP_Text currentPETextBar;

    [Header("Animation Settings")]
    [SerializeField] private float updateSpeed = 0.5f; // Velocidad de actualización de la barra

    public int basePE, maxPV;

    //Método para inicializar los textos de las barras
    public void InitializeComponent()
    {
        maxPVTextBar.text = "/" + maxPV.ToString();
        currentPVTextBar.text = maxPV.ToString();


        maxPETextBar.text = "/" + basePE.ToString();
        currentPETextBar.text = basePE.ToString();
        InitializeBars();
    }

    
    IEnumerator AnimateBar(Image bar, Image backgroundBar, float initialValue, float targetValue, TMP_Text textBar, int maxValue)
    {
        // Inicializar las barras con el valor inicial
        bar.fillAmount = initialValue;
        backgroundBar.fillAmount = initialValue;

        // Si el valor final es menor que el inicial (daño)
        if (targetValue < initialValue)
        {
            // La barra principal baja inmediatamente
            bar.fillAmount = targetValue;
            
            // La barra de fondo baja progresivamente
            float elapsedTime = 0f;
            while (elapsedTime < updateSpeed)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / updateSpeed;
                
                // Interpolar la barra de fondo del valor inicial al final
                backgroundBar.fillAmount = Mathf.Lerp(initialValue, targetValue, progress);
                
                // Actualizar el texto basado en el progreso de la barra de fondo
                if (maxValue > 0)
                {
                    int updatedTextValue = Mathf.RoundToInt(maxValue * backgroundBar.fillAmount);
                    textBar.text = updatedTextValue.ToString();
                }
                
                yield return null;
            }
        }
        else
        {
            // Si el valor final es mayor o igual (curación/aumento)
            // Ambas barras se actualizan progresivamente
            float elapsedTime = 0f;
            while (elapsedTime < updateSpeed)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / updateSpeed;
                
                // Interpolar ambas barras del valor inicial al final
                float currentFillAmount = Mathf.Lerp(initialValue, targetValue, progress);
                bar.fillAmount = currentFillAmount;
                backgroundBar.fillAmount = currentFillAmount;
                
                // Actualizar el texto basado en el progreso
                if (maxValue > 0)
                {
                    int updatedTextValue = Mathf.RoundToInt(maxValue * currentFillAmount);
                    textBar.text = updatedTextValue.ToString();
                }
                
                yield return null;
            }
        }

        
        // Actualizar el texto final correctamente
        if (maxValue > 0)
        {
            int finalTextValue = Mathf.RoundToInt(maxValue * targetValue);
            textBar.text = finalTextValue.ToString();
        }
        else
        {
            textBar.text = "0";
        }
    }

    //Método para actualizar la barra de vida
    public void UpdatePVBar(int currentPV, int maxPV)
    {
        float initialValue = PVBarBackground.fillAmount;
        float targetValue = (float)currentPV / maxPV;

        // Iniciar la animación de la barra de PS
        StartCoroutine(AnimateBar(PVBar, PVBarBackground, initialValue, targetValue, currentPVTextBar, maxPV));
    }

    //Método para actualizar la barra de PE
    public void UpdatePEBar(int currentPE, int basePE)
    {
        basePE = Mathf.Max(currentPE, basePE);
        maxPETextBar.text = "/" + basePE.ToString();
        float initialValue = PEBar.fillAmount;
        float valorFinal = basePE == 0 ? 0 : (float)currentPE / basePE;

        // Iniciar la animación de la barra de PE
        StartCoroutine(AnimateBar(PEBar, PEBar, initialValue, valorFinal, currentPETextBar, basePE));
    }


    //Método para inicializar las barras
    private void InitializeBars()
    {
        PVBar.fillAmount = 1f; // Inicializa la barra de vida al 100%
        PVBarBackground.fillAmount = 1f; // Inicializa el fondo de la barra de vida al 100%

        PEBar.fillAmount = basePE == 0 ? 0f : 1f; // Inicializa la barra de PE al 100%
    }

}
