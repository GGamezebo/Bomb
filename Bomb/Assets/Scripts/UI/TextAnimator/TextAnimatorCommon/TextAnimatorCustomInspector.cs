#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextAnimator))]
public class TextAnimatorCustomInspector : Editor
{
    private TextAnimator targetTA;

    void OnEnable()
    {
        targetTA = (TextAnimator)target;
    }

    public override void OnInspectorGUI()
    {
        targetTA.SetCurrent((AnimationName)EditorGUILayout.EnumPopup("Animation: ", targetTA.currentAnimation));

        switch (targetTA.currentAnimation)
        {
            case AnimationName.Waving:
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                targetTA.loop = EditorGUILayout.Toggle("Loop: ", targetTA.loop);
                if (targetTA.loop){EditorGUILayout.Space(18);}
                else{targetTA.duration = EditorGUILayout.Slider("Duration: ", targetTA.duration, 0, 10);}

                targetTA.wavingOffsetX = EditorGUILayout.Slider("X offset: ", targetTA.wavingOffsetX, -100f, 100f);
                targetTA.wavingOffsetY = EditorGUILayout.Slider("Y offset: ", targetTA.wavingOffsetY, -100f, 100f);
                EditorGUILayout.Space(20);

                targetTA.wavingSpeed = EditorGUILayout.Slider("Speed: ", targetTA.wavingSpeed, 0, 100f);
                targetTA.wavingDelay = EditorGUILayout.Slider("Delay: ", targetTA.wavingDelay, 0, 6.3f);
                break;

            case AnimationName.Scaling:
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                targetTA.loop = EditorGUILayout.Toggle("Loop: ", targetTA.loop);
                if (targetTA.loop) { EditorGUILayout.Space(18); }
                else { targetTA.duration = EditorGUILayout.Slider("Duration: ", targetTA.duration, 0, 10); }

                targetTA.scalingSpeed = EditorGUILayout.Slider("Speed: ", targetTA.scalingSpeed, -5f, 5f);
                targetTA.scalingDelay = EditorGUILayout.Slider("Delay: ", targetTA.scalingDelay, -5f, 5f);
                break;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(targetTA);
        }
    }
}
#endif