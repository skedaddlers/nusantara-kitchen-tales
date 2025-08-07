using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] private List<QuestionSO> questions; // Masukkan semua aset QuestionSO ke sini
    private int currentQuestionIndex = 0;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI questionTextUI;
    [SerializeField] private GameObject multipleChoicePanel;
    [SerializeField] private GameObject speechRecognitionPanel;
    [SerializeField] private Button[] optionButtons; // Drag 4 tombol pilihan ganda ke sini
    [SerializeField] private TextMeshProUGUI feedbackTextUI;
    [SerializeField] private Image normalImage;
    [SerializeField] private Image correctImage;
    [SerializeField] private Image wrongImage;
    [SerializeField] private Button homeButton; // Tombol untuk kembali ke menu utama

    [Header("Speech Recognition")]
    [SerializeField] private SpeechRecognitionController speechController; // Drag GameObject yang punya skrip ini

    void Start()
    {
        // Sembunyikan semua panel di awal
        multipleChoicePanel.SetActive(false);
        speechRecognitionPanel.SetActive(false);
        feedbackTextUI.gameObject.SetActive(false);

        // Hubungkan event dari speech controller ke fungsi kita
        if (speechController != null)
        {
            speechController.onResponse.AddListener(OnSpeechResultReceived);
        }

        normalImage.gameObject.SetActive(true);
        correctImage.gameObject.SetActive(false);
        wrongImage.gameObject.SetActive(false);

        // Mulai kuis
        ShowQuestion(currentQuestionIndex);
        homeButton.onClick.AddListener(() => SceneLoader.LoadScene("MainMenu"));
    }

    private void ShowQuestion(int index)
    {
        if (index >= questions.Count)
        {
            EndQuiz();
            return;
        }

        QuestionSO currentQuestion = questions[index];
        questionTextUI.text = currentQuestion.questionText;

        // Atur UI berdasarkan tipe pertanyaan
        if (currentQuestion.questionType == QuestionType.MultipleChoice)
        {
            SetupMultipleChoiceUI(currentQuestion);
        }
        else if (currentQuestion.questionType == QuestionType.SpeechRecognition)
        {
            SetupSpeechRecognitionUI();
        }
    }

    private void SetupMultipleChoiceUI(QuestionSO question)
    {
        speechRecognitionPanel.SetActive(false);
        multipleChoicePanel.SetActive(true);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.options[i];
            int optionIndex = i; // Penting untuk menghindari masalah closure di lambda
            optionButtons[i].onClick.RemoveAllListeners(); // Hapus listener lama
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(optionIndex));
        }
    }

    private void SetupSpeechRecognitionUI()
    {
        multipleChoicePanel.SetActive(false);
        speechRecognitionPanel.SetActive(true);
        // UI untuk speech recognition sudah diatur oleh SpeechRecognitionController
    }

    public void OnOptionSelected(int selectedIndex)
    {
        QuestionSO currentQuestion = questions[currentQuestionIndex];
        if (selectedIndex == currentQuestion.correctOptionIndex)
        {
            ShowFeedback("Benar!", true);
        }
        else
        {
            ShowFeedback("Salah, coba lagi!", false);
        }
        // Lanjut ke pertanyaan berikutnya setelah jeda singkat
        Invoke(nameof(NextQuestion), 1.5f);
    }

    // Fungsi ini akan dipanggil oleh event dari SpeechRecognitionController
    public void OnSpeechResultReceived(string recognizedText)
    {
        QuestionSO currentQuestion = questions[currentQuestionIndex];
        // Debug.Log("Speech recognized: " + recognizedText);
        // Kita bisa buat perbandingan yang lebih 'pintar' (misal: ignore case, hapus spasi)
        // bool isCorrect = string.Equals(recognizedText.Trim(), currentQuestion.correctAnswerPhrase.Trim(), System.StringComparison.OrdinalIgnoreCase);
        // Debug.Log("Comparing: " + recognizedText + " with: " + currentQuestion.correctAnswerPhrase);
        string[] correctPhrases = currentQuestion.correctAnswerPhrases;
        bool isCorrect = false;
        // split recognizedText into phrases and compare each one
        string[] recognizedPhrases = recognizedText.Split(new char[] { ' ', ',', '.' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var phrase in correctPhrases)
        {
            foreach (var recognizedPhrase in recognizedPhrases)
            {
                Debug.Log("Comparing: " + recognizedPhrase + " with: " + phrase);
                if (string.Equals(recognizedPhrase.Trim(), phrase.Trim(), System.StringComparison.OrdinalIgnoreCase))
                {
                    isCorrect = true;
                    break;
                }
            }
            if (isCorrect) break; // Stop checking if we found a match
        }
        if (isCorrect)
        {
            ShowFeedback("Benar! Kamu mengucapkan: " + recognizedText, true);
        }
        else
        {
            ShowFeedback("Hampir benar! Coba lagi. Kamu mengucapkan: " + recognizedText, false);
        }
        Invoke(nameof(NextQuestion), 2f);
    }

    private void NextQuestion()
    {
        normalImage.gameObject.SetActive(true);
        correctImage.gameObject.SetActive(false);
        wrongImage.gameObject.SetActive(false);
        currentQuestionIndex++;
        feedbackTextUI.gameObject.SetActive(false);
        ShowQuestion(currentQuestionIndex);
    }

    private void ShowFeedback(string message, bool isCorrect)
    {
        normalImage.gameObject.SetActive(false);
        correctImage.gameObject.SetActive(isCorrect);
        wrongImage.gameObject.SetActive(!isCorrect);
        feedbackTextUI.text = message;
        feedbackTextUI.color = isCorrect ? Color.green : Color.red;
        feedbackTextUI.gameObject.SetActive(true);
    }

    private void EndQuiz()
    {
        homeButton.gameObject.SetActive(true);
        questionTextUI.text = "Kuis Selesai!";
        multipleChoicePanel.SetActive(false);
        speechRecognitionPanel.SetActive(false);
    }
}