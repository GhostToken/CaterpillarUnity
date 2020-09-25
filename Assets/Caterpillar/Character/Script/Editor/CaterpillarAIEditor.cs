using UnityEditor;
using UnityEngine;
using Pathfinding;

[CustomEditor(typeof(CaterpillarAI), true)]
[CanEditMultipleObjects]
public class CaterpillarAIEditor : EditorBase
{
	protected override void Inspector()
	{
		Section("Pathfinding");
		if (PropertyField("canSearch"))
		{
			EditorGUI.indentLevel++;
			FloatField("repathRate", min: 0f);
			EditorGUI.indentLevel--;
		}

		Section("Movement");
		FloatField("speed", min: 0f);
		FloatField("endReachedDistance", min: 0f);
		FloatField("slowdownDistance", min: 0f);
		PropertyField("canMove");
		if (PropertyField("enableRotation"))
		{
			EditorGUI.indentLevel++;
			Popup("orientation", new[] { new GUIContent("ZAxisForward (for 3D games)"), new GUIContent("YAxisForward (for 2D games)") });
			FloatField("rotationSpeed", min: 0f);
			EditorGUI.indentLevel--;
		}

		if (PropertyField("interpolatePathSwitches"))
		{
			EditorGUI.indentLevel++;
			FloatField("switchPathInterpolationSpeed", min: 0f);
			EditorGUI.indentLevel--;
		}
	}
}