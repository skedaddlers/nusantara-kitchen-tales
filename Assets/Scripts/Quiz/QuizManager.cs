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

        // Mulai kuis
        ShowQuestion(currentQuestionIndex);
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
        // Kita bisa buat perbandingan yang lebih 'pintar' (misal: ignore case, hapus spasi)
        bool isCorrect = recognizedText.Equals(currentQuestion.correctAnswerPhrase, System.StringComparison.OrdinalIgnoreCase);

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
        currentQuestionIndex++;
        feedbackTextUI.gameObject.SetActive(false);
        ShowQuestion(currentQuestionIndex);
    }

    private void ShowFeedback(string message, bool isCorrect)
    {
        feedbackTextUI.text = message;
        feedbackTextUI.color = isCorrect ? Color.green : Color.red;
        feedbackTextUI.gameObject.SetActive(true);
    }

    private void EndQuiz()
    {
        questionTextUI.text = "Kuis Selesai!";
        multipleChoicePanel.SetActive(false);
        speechRecognitionPanel.SetActive(false);
    }
}