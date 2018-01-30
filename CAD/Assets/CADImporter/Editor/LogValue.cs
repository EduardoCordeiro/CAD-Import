using UnityEditor;
using UnityEngine;
using Microsoft.FSharp.Core;

public class LogValue
{
  float m_value;
  float m_min, m_max;
  string m_strNum;
  bool m_bSpecifyNumber = false;

  public LogValue(float value, float min, float max)
  {
    m_value = value;
    m_min = min;
    m_max = max;
  }

  public FSharpOption<float> Value
  {
    get
    {
      return SpecifyNumber ? FSharpOption<float>.Some(m_value) : FSharpOption<float>.None;
    }
  }

  public bool SpecifyNumber
  {
    set
    {
      m_bSpecifyNumber = value;
    }
    get
    {
      return m_bSpecifyNumber;
    }
  }

  public float Min
  {
    get
    {
      return m_min;
    }
  }
  public float Max
  {
    get
    {
      return m_max;
    }
  }

  public float ToSliderValue(float n)
  {
    return Mathf.Log10(n);
  }
  public float ToSliderValue()
  {
    return ToSliderValue(m_value);
  }
  public float OfSliderValue(float sliderValue)
  {
    m_value = (int)Mathf.Pow(10.0f, sliderValue);
    return m_value;
  }
  public override string ToString()
  {
    if (m_value >= 1000000) {
      float f = m_value / 1000000.0f;
      return string.Format("{0:0.0M}", f);
    }
    else if (m_value >= 1000) {
      float f = m_value / 1000.0f;
      return string.Format("{0:0.0K}", f);
    }
    else {
      return string.Format("{0}", m_value);
    }
  }
  public float Parse(string str)
  {
    if (str.EndsWith("M")) {
      var s = str.Split('M')[0];
      m_value = (float.Parse(s) * 1000000.0f);
    }
    else if (str.EndsWith("K")) {
      var s = str.Split('K')[0];
      m_value = (float.Parse(s) * 1000.0f);
    }
    else {
      m_value = float.Parse(str);
    }
    return m_value;
  }

  public void DrawGUI(string sLabel, int nIndent = 0)
  {
    EditorGUILayout.BeginHorizontal();
    {
      if (nIndent > 0) {
        GUILayout.Space(nIndent * 20);
      }

      SpecifyNumber = GUILayout.Toggle(SpecifyNumber, sLabel, GUILayout.ExpandWidth(false));

      EditorGUI.BeginDisabledGroup(!m_bSpecifyNumber);
      {
        var sliderValue = GUILayout.HorizontalSlider(ToSliderValue(), ToSliderValue(Min), ToSliderValue(Max), GUILayout.MinWidth(100));
        OfSliderValue(sliderValue);
      }
      EditorGUI.EndDisabledGroup();

      EditorGUI.BeginDisabledGroup(true);
      {
        m_strNum = ToString();
        m_strNum = GUILayout.TextField(m_strNum, GUILayout.MaxWidth(50.0f), GUILayout.MinWidth(10.0f));
        Parse(m_strNum);
      }
      EditorGUI.EndDisabledGroup();
    }
    EditorGUILayout.EndHorizontal();
  }
};
