using TMPro;
using UnityEngine;

public class TA_Waving : ITextAnimation
{
    private TextAnimator _textAnimator;
    private TMP_Text _tmpText;
    private TMP_TextInfo _tmpTextInfo;

    private Vector3[] _vertices;
    private Vector3 _offset;
    private Vector3 _scale;

    private int _charCount;
    private int _materialIndex;
    private int _vertexIndex;

    public TA_Waving(TextAnimator textAnimator)
    {
        _textAnimator = textAnimator;
        _tmpText = _textAnimator.textField;
    }

    public void Play()
    {
        _tmpText.ForceMeshUpdate();

        _tmpTextInfo = _tmpText.textInfo;
        _charCount = _tmpTextInfo.characterCount;

        if (_charCount > 0)
        {
            for (int i = 0; i < _charCount; i++)
            {
                if (!_tmpTextInfo.characterInfo[i].isVisible)
                    continue;

                _materialIndex = _tmpTextInfo.characterInfo[i].materialReferenceIndex;
                _vertices = _tmpTextInfo.meshInfo[_materialIndex].vertices;
                _vertexIndex = _tmpTextInfo.characterInfo[i].vertexIndex;

                // move char
                _offset = new Vector3(
                    Mathf.Sin(Time.time * _textAnimator.wavingSpeed + i * _textAnimator.wavingDelay) * _textAnimator.wavingOffsetX,
                    Mathf.Sin(Time.time * _textAnimator.wavingSpeed + i * _textAnimator.wavingDelay) * _textAnimator.wavingOffsetY,
                    0);

                for (int j = 0; j < 4; j++)
                {
                    _vertices[_vertexIndex + j] += _offset;
                }
            }

            // update mesh
            for (int i = 0; i < _tmpTextInfo.meshInfo.Length; i++)
            {
                _tmpText.mesh.vertices = _tmpTextInfo.meshInfo[i].vertices;
                _tmpText.UpdateGeometry(_tmpTextInfo.meshInfo[i].mesh, i);
            }
        }
    }
}
