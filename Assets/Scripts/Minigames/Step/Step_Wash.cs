using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Step_Wash : Step
{
    public Button tombolKran;
    public Image airKran;

    private ResepDataSO resep { get; set; }
    private Bahan bahanDiperlukan;

    void Start()
    {
        isActive = true; // Set the step as active
        airKran.enabled = false; // Initially disable the water image
        resep = GameData.ResepDipilih; // Get the current recipe data

        LoadAlatDanBahan(); // Load the required tools and ingredients for the step


        tombolKran.onClick.AddListener(OnKranClicked); // Add listener to the button
    }

    public override void DisableStep()
    {
        isActive = false;
        // airKran.enabled = false; // Disable the water image when the step is disabled
        tombolKran.onClick.RemoveListener(OnKranClicked); // Remove the button listener
    }

    private void OnKranClicked()
    {
        if (isActive)
        {
            airKran.enabled = true; // Enable the water image when the button is clicked
                                    // Optionally, you can add a sound effect or animation here
                                    // Animate the water image to appear with a fade-in effect
            airKran.DOFade(1f, 0.5f).SetEase(Ease.InOutQuad);
            // rotate button kran 90 degrees
            tombolKran.transform.DORotate(new Vector3(0, 0, -90), 0.5f).SetEase(Ease.OutBack); 

            // taruh air ke paling depan    
            airKran.transform.SetAsLastSibling();
            GameplayManager.Instance.NextStep(); // Proceed to the next step in the game
        }
    }

    private void LoadAlatDanBahan()
    {
        int currentStep = GameplayManager.Instance.CurrentStep;
        if (resep.langkahMasak[currentStep].addToExisting)
        {
            currentStep--;
        }

        var alatDiperlukan = resep.langkahMasak[currentStep].alatDiperlukan;
        if (alatDiperlukan == null || alatDiperlukan.Length == 0)
        {
            Debug.LogError("Alat tidak ditemukan untuk langkah ini!");
            return;
        }
        foreach (var alat in alatDiperlukan)
        {
            if (alat == null)
            {
                Debug.LogError("Alat tidak ditemukan dalam resep!");
                continue;
            }
            // Instantiate alat
            Alat item = Instantiate(alat, transform);
            item.name = alat.namaAlat;
            item.GetComponent<Image>().sprite = alat.gambarAlat;
            Utilz.SetSizeNormalized(item.GetComponent<RectTransform>(), alat.gambarAlat, 500f, 500f);
            // Set posisi alat
            float posisiAlat = Screen.width / (alatDiperlukan.Length + 1);
            item.transform.localPosition = new Vector3(-100f, -100f, 0);
        }

        var bahanDiperlukan = resep.langkahMasak[currentStep].bahanDiperlukan;
        if (bahanDiperlukan == null || bahanDiperlukan.Length == 0)
        {
            Debug.LogError("Bahan tidak ditemukan untuk langkah ini!");
            return;
        }
        foreach (var bahan in bahanDiperlukan)
        {
            if (bahan == null)
            {
                Debug.LogError("Bahan tidak ditemukan dalam resep!");
                continue;
            }
            // Instantiate bahan
            Bahan item = Instantiate(bahan, transform);
            item.name = bahan.namaBahan;
            item.GetComponent<Image>().sprite = bahan.gambarBahan;
            // set native size of the image
            item.GetComponent<Image>().SetNativeSize();
            Utilz.SetSizeNormalized(item.GetComponent<RectTransform>(), bahan.gambarBahan, 400f, 400f);
            // Set posisi bahan
            float posisiBahan = Screen.width / (bahanDiperlukan.Length + 1);
            item.transform.localPosition = new Vector3(-100f, -100f, 0);
        }
        
    }

    
}