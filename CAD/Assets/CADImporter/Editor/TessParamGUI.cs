using System;
using UnityEditor;
using UnityEngine;
using Microsoft.FSharp.Core;
using MonoRAILProxy;

public class TessParamGUI : MonoRAILProxy.TessParam
{
  // 簡単設定
  readonly TessellationTolerance[] m_easyToleranceTable;
  struct EasySetting
  {
    public int smoothAngle;      // STLメッシュの隣り合う三角パッチの角度がこれ以下なら滑らかに繋ぐ
    public int tessAngle;        // 最大角度
    public int tessApectRatio;   // 最大縦横比
  };
  EasySetting[] m_easySettingTable = {
    new EasySetting() { smoothAngle = 40, tessAngle =  5, tessApectRatio = 24 },
    new EasySetting() { smoothAngle = 35, tessAngle =  8, tessApectRatio = 32 },
    new EasySetting() { smoothAngle = 30, tessAngle = 10, tessApectRatio = 48 },
    new EasySetting() { smoothAngle = 25, tessAngle = 12, tessApectRatio = 64 },
    new EasySetting() { smoothAngle = 20, tessAngle = 15, tessApectRatio =  0 },
    new EasySetting() { smoothAngle = 15, tessAngle = 20, tessApectRatio =  0 },
    new EasySetting() { smoothAngle = 10, tessAngle = 23, tessApectRatio =  0 },
    new EasySetting() { smoothAngle =  5, tessAngle = 26, tessApectRatio =  0 },
    new EasySetting() { smoothAngle =  0, tessAngle = 30, tessApectRatio =  0 },
  };
  int m_nEasyTessLevel = 0;
  int m_nLastEasyTessLevel = 0;
  bool m_isAdvancedSetting = false;

  // TessAngle
  const int m_tessAngleMin = 5;
  const int m_tessAngleMax = 30;
  // Acpect Ratio
  const int m_maxAspectRatio_Min = 4;
  const int m_maxAspectRatio_Max = 64;
  int m_maxAspectRatio = 8;  // (初期値＆記憶保持用)
  // Min.Length
  const double m_minLength_Min = 0.001; // -> log10: -3
  const double m_minLength_Max = 10.0;   // -> log10: 1
  double m_minLength = 0.01;  // (初期値＆記憶保持用)
  // Max.Length
  readonly LogValue m_MaxLength;
  const float m_maxLength_Min = 1.0f;
  const float m_maxLength_Max = 10000.0f;
  const float m_maxLength_Def = 100.0f;
  // 最大誤差
  const float m_deviation_Min = 0.0f;
  const float m_deviation_Max = 10.0f;
  // 目標頂点数
  readonly LogValue m_NumVertices;
  const int m_numVertices_Min = 1000;
  const int m_numVertices_Max = 10000000;
  const int m_numVertices_Def = 65000;
  // バウンディングボックスの辺の最小値
  readonly LogValue m_MinBoxLength;
  const float m_minBoxLength_Min = 1.0f;
  const float m_minBoxLength_Max = 1000.0f;
  const float m_minBoxLength_Def = 1.0f;
  FSharpOption<float> m_minBoxLength = FSharpOption<float>.None;
  // ヒーリングスイッチ
  bool m_isEnableHealing = false;

  public TessParamGUI() : base()
  {
    m_MaxLength = new LogValue(m_maxLength_Def, m_maxLength_Min, m_maxLength_Max);
    m_NumVertices = new LogValue(m_numVertices_Def, m_numVertices_Min, m_numVertices_Max);
    m_MinBoxLength = new LogValue(m_minBoxLength_Def, m_minBoxLength_Min, m_minBoxLength_Max);

    // 簡単設定テーブル
    int nTableSize = m_easySettingTable.Length;
    m_easyToleranceTable = new TessellationTolerance[nTableSize];
    for (int i = 0; i < nTableSize; ++i) {
      var easySetting = m_easySettingTable[i];
      var tole = TessellationTolerance.Default;
      tole.TessAngle = MoNo.Angle.FromDegree(easySetting.tessAngle);
      tole.TessAspectRatio = (easySetting.tessApectRatio == 0)
                             ? FSharpOption<TessellationAspectRatio>.None
                             : FSharpOption<TessellationAspectRatio>.Some(
                                 new TessellationAspectRatio() {
                                   MaxRatio = easySetting.tessApectRatio,
                                   MinLength = FSharpOption<double>.None
                                 } );
      m_easyToleranceTable[i] = tole;
    }

    // 初期値は真ん中で
    m_nEasyTessLevel = m_nLastEasyTessLevel = nTableSize / 2;
    var src = m_easyToleranceTable[m_nEasyTessLevel];
    m_tolerance = new TessellationTolerance() {
      TessAngle = src.TessAngle,
      TessAspectRatio = src.TessAspectRatio,
      TessMaxLength = src.TessMaxLength,
      TessDeviation = src.TessDeviation
    };
  }

  public override int SmoothAngle()
  {
    return m_easySettingTable[m_nEasyTessLevel].smoothAngle;
  }

  public override bool IsEnableHealing()
  {
    return m_isEnableHealing;
  }

  public override int NumVertices()
  {
    return FSharpOption<float>.get_IsSome(m_NumVertices.Value) ? (int)m_NumVertices.Value.Value : 0;
  }

  public override double MinBoxLength()
  {
    return FSharpOption<float>.get_IsSome(m_MinBoxLength.Value) ? (double)m_MinBoxLength.Value.Value : double.NegativeInfinity;
  }

  public void DrawGUI()
  {
    EditorGUILayout.BeginVertical(GUI.skin.box);
    EditorGUILayout.LabelField("Tessellation");

//    m_NumVertices.DrawGUI("目標頂点数");
    m_NumVertices.DrawGUI("Target Vertices Count");

    // 簡単設定
    GUILayout.BeginHorizontal();
//    GUILayout.Label("滑らか", GUILayout.ExpandWidth(false));
    GUILayout.Label("Smooth", GUILayout.ExpandWidth(false));
    m_nEasyTessLevel = (int)GUILayout.HorizontalSlider(m_nLastEasyTessLevel, 0, m_easySettingTable.Length - 1, GUILayout.MinWidth(100));
    if (m_nEasyTessLevel != m_nLastEasyTessLevel) {
      var src = m_easyToleranceTable[m_nEasyTessLevel];
      m_tolerance.TessAngle = src.TessAngle;
      m_tolerance.TessAspectRatio = src.TessAspectRatio;
      m_tolerance.TessDeviation = src.TessDeviation;
      m_tolerance.TessMaxLength = src.TessMaxLength;
      m_nLastEasyTessLevel = m_nEasyTessLevel;
    }
//    GUILayout.Label("粗い", GUILayout.ExpandWidth(false));
    GUILayout.Label("Rough", GUILayout.ExpandWidth(false));
    GUILayout.EndHorizontal();

//    m_isAdvancedSetting = EditorGUILayout.ToggleLeft("詳細設定", m_isAdvancedSetting);
    m_isAdvancedSetting = EditorGUILayout.ToggleLeft("Advanced Settings", m_isAdvancedSetting);
    EditorGUI.indentLevel++;
    if (m_isAdvancedSetting) {
      // TessAngle
      var tessAngle = (int)m_tolerance.TessAngle.Degree;
      EditorGUILayout.BeginHorizontal();
      GUILayout.Space(1 * 20);
//      GUILayout.Label("最大角度(°)", GUILayout.ExpandWidth(false));
      GUILayout.Label("Max Angle(deg)", GUILayout.ExpandWidth(false));
      GUILayout.FlexibleSpace();
      tessAngle = EditorGUILayout.IntSlider("", tessAngle, m_tessAngleMin, m_tessAngleMax, GUILayout.MaxWidth(300));
      EditorGUILayout.EndHorizontal();
      m_tolerance.TessAngle.Degree = tessAngle;

      // Aspect Ratio
      EditorGUILayout.BeginHorizontal();
      GUILayout.Space(1 * 20);
      bool isTessAspectRatioSpecified = FSharpOption<TessellationAspectRatio>.get_IsSome(m_tolerance.TessAspectRatio);
      bool isTessMinLengthSpecified;
      if (isTessAspectRatioSpecified) {
        m_maxAspectRatio = (int)m_tolerance.TessAspectRatio.Value.MaxRatio;
        isTessMinLengthSpecified = FSharpOption<double>.get_IsSome(m_tolerance.TessAspectRatio.Value.MinLength);
        if (isTessMinLengthSpecified) {
          m_minLength = m_tolerance.TessAspectRatio.Value.MinLength.Value;
        }
      }
      else {
        isTessMinLengthSpecified = false;
      }
//      isTessAspectRatioSpecified = GUILayout.Toggle(isTessAspectRatioSpecified, "最大縦横比(1:n)", GUILayout.ExpandWidth(false));
      isTessAspectRatioSpecified = GUILayout.Toggle(isTessAspectRatioSpecified, "Max Aspect Ratio(1:n)", GUILayout.ExpandWidth(false));

      EditorGUI.BeginDisabledGroup(!isTessAspectRatioSpecified);
      {
        GUILayout.FlexibleSpace();
        m_maxAspectRatio = EditorGUILayout.IntSlider("", m_maxAspectRatio, m_maxAspectRatio_Min, m_maxAspectRatio_Max, GUILayout.MaxWidth(300));
        EditorGUILayout.EndHorizontal();

        if (isTessAspectRatioSpecified) {
          var newAspectRatio = new TessellationAspectRatio() {
            MaxRatio = m_maxAspectRatio,
            MinLength = (isTessMinLengthSpecified) ? FSharpOption<double>.Some(m_minLength) : FSharpOption<double>.None
          };
          m_tolerance.TessAspectRatio = FSharpOption<TessellationAspectRatio>.Some(newAspectRatio);
        }
        else {
          m_tolerance.TessAspectRatio = FSharpOption<TessellationAspectRatio>.None;
        }

        // 最小エッジ長さ - どれかのエッジが最小エッジ長さより短い場合、メッシュ面の分割はそれ以上行われない
        EditorGUILayout.BeginHorizontal();
        {
          GUILayout.Space(2 * 20);
          isTessMinLengthSpecified = isTessAspectRatioSpecified ? FSharpOption<double>.get_IsSome(m_tolerance.TessAspectRatio.Value.MinLength) : false;
          if (isTessMinLengthSpecified) {
            m_minLength = m_tolerance.TessAspectRatio.Value.MinLength.Value;
          }

//          isTessMinLengthSpecified = GUILayout.Toggle(isTessMinLengthSpecified, "最小エッジ長さ(mm)", GUILayout.ExpandWidth(false));
          isTessMinLengthSpecified = GUILayout.Toggle(isTessMinLengthSpecified, "Min Edge Length(mm)", GUILayout.ExpandWidth(false));

          EditorGUI.BeginDisabledGroup(!isTessMinLengthSpecified);
          {
            var sliderMin = (int)Math.Log10(m_minLength_Min);
            var sliderMax = (int)Math.Log10(m_minLength_Max);
            var sliderValue = (int)Math.Log10(m_minLength);
            sliderValue = (int)GUILayout.HorizontalSlider(sliderValue - sliderMin, 0, sliderMax - sliderMin, GUILayout.MinWidth(100)) + sliderMin;
            m_minLength = Math.Pow(10, sliderValue);
            GUILayout.TextField(m_minLength.ToString(), GUILayout.MaxWidth(50.0f), GUILayout.MinWidth(10.0f));
            if (isTessAspectRatioSpecified) {
              if (isTessMinLengthSpecified) {
                var minLength = FSharpOption<double>.Some(m_minLength);
                var aspectRatio = new TessellationAspectRatio() { MaxRatio = m_tolerance.TessAspectRatio.Value.MaxRatio, MinLength = minLength };
                m_tolerance.TessAspectRatio = FSharpOption<TessellationAspectRatio>.Some(aspectRatio);
              }
              else {
                var aspectRatio = new TessellationAspectRatio() { MaxRatio = m_maxAspectRatio, MinLength = FSharpOption<double>.None };
                m_tolerance.TessAspectRatio = FSharpOption<TessellationAspectRatio>.Some(aspectRatio);
              }
            }
          }
          EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndHorizontal();
      }
      EditorGUI.EndDisabledGroup();

      // 最大エッジ長さ - すべてのポリゴンエッジがこの値より短くなるまで、ポリゴンは更に分割される
      m_MaxLength.SpecifyNumber = FSharpOption<Tuple<double, double>>.get_IsSome(m_tolerance.TessMaxLength);
//      m_MaxLength.DrawGUI("最大エッジ長さ(mm)", 1);
      m_MaxLength.DrawGUI("Max Edge Length(mm)", 1);
      m_tolerance.TessMaxLength = (m_MaxLength.SpecifyNumber)
                                  ? FSharpOption<Tuple<double, double>>.Some(new Tuple<double, double>(m_MaxLength.Value.Value, m_MaxLength.Value.Value))
                                  : FSharpOption<Tuple<double, double>>.None;

      // 最大誤差
      EditorGUILayout.BeginHorizontal();
      GUILayout.Space(1 * 20);
      bool isInfinity = double.IsInfinity(m_tolerance.TessDeviation);
//      isInfinity = ! GUILayout.Toggle(! isInfinity, "最大誤差(mm)", GUILayout.ExpandWidth(false));
      isInfinity = !GUILayout.Toggle(!isInfinity, "Max Deviation(mm)", GUILayout.ExpandWidth(false));
      GUILayout.FlexibleSpace();
      double deviation = m_tolerance.TessDeviation;
      m_tolerance.TessDeviation = isInfinity ? double.PositiveInfinity : deviation;
      EditorGUI.BeginDisabledGroup(isInfinity);
      {
        deviation = EditorGUILayout.Slider("", (float)deviation, m_deviation_Min, m_deviation_Max);
        if (! isInfinity) {
          m_tolerance.TessDeviation = deviation;
        }
      }
      EditorGUI.EndDisabledGroup();
      EditorGUILayout.EndHorizontal();

      // バウンディングボックスの辺の最小値
      m_MinBoxLength.SpecifyNumber = FSharpOption<float>.get_IsSome(m_minBoxLength);
//      m_MinBoxLength.DrawGUI("無視サイズ(mm)", 1);
      m_MinBoxLength.DrawGUI("Discarding Threshold(mm)", 1);
      m_minBoxLength = (m_MinBoxLength.SpecifyNumber)
                        ? FSharpOption<float>.Some(m_MinBoxLength.Value.Value)
                        : FSharpOption<float>.None;

      // ヒーリングスイッチ
      EditorGUILayout.BeginHorizontal();
      GUILayout.Space(1 * 20);
      m_isEnableHealing = GUILayout.Toggle(m_isEnableHealing, "Mesh Healing", GUILayout.ExpandWidth(false));
      EditorGUILayout.EndHorizontal();

    } // end if (m_isAdvancedSetting)
    EditorGUI.indentLevel--;
    EditorGUILayout.EndVertical();
  }
}
