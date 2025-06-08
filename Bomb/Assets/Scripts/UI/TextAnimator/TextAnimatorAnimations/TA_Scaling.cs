using TMPro;
using UnityEngine;

public class TA_Scaling : ITextAnimation
{
    private TextAnimator _textAnimator;
    private TMP_Text _tmpText;
    private TMP_TextInfo textInfo;

    private Vector3[][] originalVertices;
    private float[] charTimers;
    private float globalTimer;
    private bool initialized = false;

    public TA_Scaling(TextAnimator textAnimator)
    {
        _textAnimator = textAnimator;
        _tmpText = _textAnimator.textField;
        Init();
    }

    void Init()
    {
        if (_tmpText == null) return;

        _tmpText.ForceMeshUpdate();
        textInfo = _tmpText.textInfo;
        int charCount = textInfo.characterCount;

        // Инициализация таймеров для каждого символа
        charTimers = new float[charCount];
        for (int i = 0; i < charCount; i++)
        {
            charTimers[i] = -_textAnimator.scalingDelay * i; // Отрицательные значения для задержки
        }

        // Сохраняем оригинальные вершины
        originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            int vertexCount = textInfo.meshInfo[i].vertices.Length;
            originalVertices[i] = new Vector3[vertexCount];
            System.Array.Copy(textInfo.meshInfo[i].vertices, originalVertices[i], vertexCount);
        }

        globalTimer = 0f;
        initialized = true;
    }

    public void Play()
    {
        if (!initialized || _tmpText == null) return;
        if (textInfo.characterCount != charTimers.Length) Init();

        globalTimer += Time.deltaTime;
        bool anyCharacterAnimating = false;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            charTimers[i] += Time.deltaTime;

            // Обработка анимации для символа
            if (charTimers[i] >= 0 && charTimers[i] <= _textAnimator.scalingSpeed)
            {
                AnimateCharacterScale(i, charTimers[i], _textAnimator.scalingSpeed);
                anyCharacterAnimating = true;
            }
            // Сброс таймера для повторения анимации
            else if (charTimers[i] > _textAnimator.scalingSpeed)
            {
                // Возвращаем символ в исходное состояние
                ResetCharacter(i);
                charTimers[i] = -_textAnimator.scalingDelay;
            }
        }

        // Обновляем меш только если есть анимируемые символы
        if (anyCharacterAnimating)
        {
            _tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
    }

    void AnimateCharacterScale(int charIndex, float timer, float speed)
    {
        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        var vertices = textInfo.meshInfo[materialIndex].vertices;
        float progress = Mathf.Clamp01(timer / speed);
        float scale = progress; // Линейная анимация от 0 до 1

        // Центр символа
        Vector3 charMidBaseline = (originalVertices[materialIndex][vertexIndex] +
                                  originalVertices[materialIndex][vertexIndex + 2]) / 2;

        for (int j = 0; j < 4; j++)
        {
            Vector3 orig = originalVertices[materialIndex][vertexIndex + j];
            Vector3 offset = orig - charMidBaseline;
            vertices[vertexIndex + j] = charMidBaseline + offset * scale;
        }
    }

    void ResetCharacter(int charIndex)
    {
        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
        var vertices = textInfo.meshInfo[materialIndex].vertices;

        // Восстанавливаем оригинальные вершины
        for (int j = 0; j < 4; j++)
        {
            vertices[vertexIndex + j] = originalVertices[materialIndex][vertexIndex + j];
        }
    }
}
