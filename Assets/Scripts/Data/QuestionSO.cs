using UnityEngine;

[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/Question")]
public class QuestionSO : ScriptableObject
{
    [TextArea(3, 10)]
    public string questionText; // Teks pertanyaan, misal: "Apa bahan utama..."

    public QuestionType questionType; // Tipe pertanyaan (Pilihan Ganda atau Suara)

    // -- Opsi untuk Pilihan Ganda --
    [Header("Multiple Choice Options")]
    public string[] options = new string[4];
    public int correctOptionIndex;

    // -- Opsi untuk Pengenalan Suara --
    [Header("Speech Recognition Option")]
    public string correctAnswerPhrase; // Frasa yang benar untuk diucapkan
}

public enum QuestionType
{
    MultipleChoice,
    SpeechRecognition
}