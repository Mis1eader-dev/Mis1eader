/*
To use a feature from the following three #define lines below, uncomment the forward slashes "//" that come before #define preprocessors, and to disable one, comment it with two forward slashes.
Keep in mind that only one feature must be used at a time, or the highest one in the order will take priority.
Also it is only possible to switch to a lower feature and not the other way around, for example if you enable ENABLE_UNITY_TEXT and go back to ENABLE_REFLECTION, then you will have to re-assign the text mesh field manually, or you can re-import the package.
As for ENABLE_TEXT_MESH_PRO, you will have to remove existing text components and add Text Mesh Pro components to the prefabs manually, and then assign the text fields.
The following scripts must use the same features as each other:
	1. Assets/Mis1eader/Gauge/GaugeBodyPart.cs
	2. Assets/Mis1eader/Gauge/GaugeSystem.cs
	3. Assets/Mis1eader/Gauge/Editor/Gauge Body Part.cs
	4. Assets/Mis1eader/Gauge/Editor/Gauge System.cs
*/
#define ENABLE_REFLECTION //Uses reflections, it works on every component that has a text field or property, but when the text or color of the text component changes continuously, it produces garbage that triggers the garbage collection in order to make the changes, results in slight performance loss.
//#define ENABLE_UNITY_TEXT //Uses Unity's built-in text components, Text and Text Mesh, it directly modifies text and color properties of these components.
//#define ENABLE_TEXT_MESH_PRO //Uses Text Mesh Pro, it has components for both 2D and 3D text.

#if ENABLE_REFLECTION
#undef ENABLE_UNITY_TEXT
#undef ENABLE_TEXT_MESH_PRO
#else
#if	ENABLE_UNITY_TEXT
#undef ENABLE_TEXT_MESH_PRO
#else
#if	!ENABLE_TEXT_MESH_PRO
#define ENABLE_REFLECTION
#endif
#endif
#endif
namespace Mis1eader.Gauge
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Gauge/Gauge System",0),ExecuteInEditMode]
	public class GaugeSystem : MonoBehaviour
	{
		public enum Dimension : byte {TwoDimensional,ThreeDimensional}
		public enum Type : byte {Analog,Digital}
		public enum TwoDimensional : byte {SpriteRenderer = 0,Image = 1,RawImage = 2}
		public enum ThreeDimensional : byte {SpriteRenderer = 0,MeshRenderer = 3}
		public enum Digital : byte {Activation,Color}
		public enum Range : byte {None,Clamp,Teleport}
		public enum Integerize : byte {DontIntegerize,Cast,Floor,Round,Ceil}
		[System.Serializable] public class Shape
		{
			public enum Type : byte {Triangle = 3,Square = 4,Pentagon = 5,Hexagon = 6,Heptagon = 7,Octagon = 8,Nonagon = 9,Decagon = 10}
			public Type type = Type.Square;
			public bool straightenBorder = false;
			[Range(0,100)] public byte tolerance = 100;
			[Range(0,100)] public byte starness = 0;
			[Range(0,100)] public byte starSharpness = 60;
			public float orientation = 0F;
			public void Update ()
			{
				if(tolerance > 100)tolerance = 100;
				if(starness > 100)starness = 100;
				if(starSharpness > 100)starSharpness = 100;
				if(orientation <= -360F)orientation = orientation % -360F;
				else if(orientation >= 360F)orientation = orientation % 360F;
			}
			public float Offset (float angle,float radius,float horizontalTickScale,float tickPivot)
			{
				if(tolerance == 0)return 0F;
				byte type = (byte)this.type;
				if(type >= 3 && type <= 10)
				{
					float starness = this.starness * 0.01F;
					float segment = Mathf.PI / type;
					float pointness = (Mathf.Cos(segment) / Mathf.Tan(segment)) * starSharpness * 0.006F;
					angle = Mathf.PingPong(Mathf.Abs(type % 2 == 1 ? 180F + angle - orientation : angle - orientation - segment * Mathf.Rad2Deg * starness) * Mathf.Deg2Rad,segment);
					radius = radius / (Inside(segment) * Mathf.Lerp(1F,Inside(segment,pointness),starness) * Mathf.Cos(angle) - Inside(segment,pointness) * starness * pointness * Mathf.Tan(angle)) - radius + (!straightenBorder ? 0F : Mathf.Tan(angle) * horizontalTickScale * (0.5F - tickPivot));
				}
				return radius * tolerance * 0.01F;
			}
			private static float Inside (float angle) {return 1F / Mathf.Cos(angle);}
			private static float Inside (float angle,float pointness) {return 1F / (1F - pointness * Mathf.Tan(angle));}
			public void SetType (Type value) {type = value;}
			public void SetType (int value) {type = (Type)value;}
			public void SetStraightenBorder (bool value) {straightenBorder = value;}
			public void SetTolerance (byte value) {tolerance = value;}
			public void SetStarness (byte value) {starness = value;}
			public void SetStarSharpness (byte value) {starSharpness = value;}
			public void SetOrientation (float value) {orientation = value;}
		}
		[System.Serializable] public class AdditionalValue
		{
			public string name = string.Empty;
			public float minimumValue = 0F;
			public Range range = Range.Clamp;
			public float value = 0F;
			public float maximumValue = 100F;
			public AdditionalValue () {}
			public AdditionalValue (float minimumValue = 0F,float value = 0F,float maximumValue = 100F)
			{
				this.minimumValue = minimumValue;
				this.value = value;
				this.maximumValue = maximumValue;
			}
			public void Update ()
			{
				if(minimumValue < float.MinValue)minimumValue = float.MinValue;
				else if(minimumValue > float.MaxValue)minimumValue = float.MaxValue;
				if(range == Range.None)
				{
					if(value < float.MinValue)value = float.MinValue;
					else if(value > float.MaxValue)value = float.MaxValue;
				}
				else
				{
					float minimum = minimumValue <= maximumValue ? minimumValue : maximumValue;
					float maximum = maximumValue >= minimumValue ? maximumValue : minimumValue;
					if(value < minimum)value = range == Range.Clamp ? minimum : (minimum != maximum ? maximum - (minimum - value) % (maximum - minimum) : minimum);
					else if(value > maximum)value = range == Range.Clamp ? maximum : (minimum != maximum ? minimum - (maximum - value) % (minimum - maximum) : minimum);
				}
				if(maximumValue < float.MinValue)maximumValue = float.MinValue;
				else if(maximumValue > float.MaxValue)maximumValue = float.MaxValue;
			}
			public void SetName (string value) {name = value;}
			public void SetMinimumValue (float value) {minimumValue = value;}
			public void SetRange (Range value) {range = value;}
			public void SetRange (int value) {range = (Range)value;}
			public void SetValue (float value) {this.value = value;}
			public void SetMaximumValue (float value) {maximumValue = value;}
		}
		[System.Serializable] public class Needle
		{
			public Integerize integerize = Integerize.DontIntegerize;
			[Tooltip("Index of additional values to use, leave it as -1 if you don't know what it does, -1 means to use the built-in value, minimum value and maximum value.")]
			public sbyte index = -1;
			public Material material = null;
			public float pivotPoint = 0F;
			public float pivotOffset = 0F;
			public float pivotOffsetFactor = 1F;
			public float headOffset = -0.065F;
			public float headOffsetFactor = 1F;
			public Vector2 scale = new Vector2(0.015F,0.5F);
			public Vector2 scaleMultiplier = Vector2.one;
			public float scaleFactor = 1F;
			public float pivotScale = 1F;
			public bool overrideColor = true;
			public Color color = Color.red;
			[HideInInspector] public GaugeBodyPart needle = null;
			[HideInInspector] public GameObject tracker = null;
			[HideInInspector] public bool updateNeedleTransform = true,updateNeedleColor = true;
			public Needle () {}
			public Needle (sbyte index) {this.index = index;}
			public void Update (byte component,Material sampleMaterial)
			{
				if(pivotPoint < 0F)pivotPoint = 0F;
				else if(pivotPoint > 0.5F)pivotPoint = 0.5F;
				if(component == 3)
				{
					if(!material)
					{
						if(sampleMaterial)material = new Material(sampleMaterial);
						else
						{
							material = new Material(Shader.Find("Standard"));
							material.SetFloat("_Glossiness",0F);
							material.SetFloat("_Metallic",0F);
						}
						material.name = "Needle";
					}
				}
				else if(material)
				{
					#if UNITY_EDITOR
					if(!Application.isPlaying)DestroyImmediate(material);
					else
					#endif
					Destroy(material);
				}
			}
			public void Update (
			#if UNITY_EDITOR
			bool run,
			#endif
			bool updates,byte component,Material sampleMaterial,float orientation,float minimumAngle,float maximumAngle,float weight,float minimumValue,float value,float maximumValue,GameObject prefab,float factor,float linear,Transform transform)
			{
				if(pivotPoint < 0F)pivotPoint = 0F;
				else if(pivotPoint > 0.5F)pivotPoint = 0.5F;
				if(component == 3)
				{
					if(!material)
					{
						if(sampleMaterial)material = new Material(sampleMaterial);
						else
						{
							material = new Material(Shader.Find("Standard"));
							material.SetFloat("_Glossiness",0F);
							material.SetFloat("_Metallic",0F);
						}
						material.name = "Needle";
					}
				}
				else if(material)
				{
					#if UNITY_EDITOR
					if(!Application.isPlaying)DestroyImmediate(material);
					else
					#endif
					Destroy(material);
				}
				#if UNITY_EDITOR
				if(run)
				{
					#endif
					if(updates && tracker != prefab)
					{
						tracker = prefab;
						if(needle)
						{
							#if UNITY_EDITOR
							if(!Application.isPlaying)DestroyImmediate(needle.gameObject);
							else
							#endif
							Destroy(needle.gameObject);
							needle = null;
						}
					}
					if(updateNeedleColor && component == 3 && overrideColor)
						ColorHandler(material,color);
					if(!needle)
					{
						if(prefab)
						{
							if(!prefab.GetComponent<GaugeBodyPart>())
							{
								#if UNITY_EDITOR
								Debug.LogError("No Gauge Body Part component was found on the needle prefab",prefab);
								#endif
								return;
							}
							needle = Instantiate(prefab).GetComponent<GaugeBodyPart>();
							needle.transform.name = "Needle";
							needle.transform.SetParent(transform,false);
							needle.transform.SetAsLastSibling();
						}
						else
						{
							#if UNITY_EDITOR
							Debug.LogError("No needle prefab is assigned",transform);
							#endif
							return;
						}
						updateNeedleTransform = true;
						updateNeedleColor = true;
					}
					if(updateNeedleTransform)
					{
						Vector2 position = (Vector2.up * (pivotOffset * pivotOffsetFactor)) * scaleFactor * factor;
						Vector2 scale = (new Vector2(this.scale.x * scaleMultiplier.x,this.scale.y * scaleMultiplier.y) + Vector2.up * (headOffset * headOffsetFactor - pivotOffset * pivotOffsetFactor)) * scaleFactor * factor;
						if(integerize != Integerize.DontIntegerize)
							value = integerize == Integerize.Cast ? (int)value :
									(integerize == Integerize.Floor ? Mathf.Floor(value) :
									(integerize == Integerize.Round ? Mathf.Round(value) : Mathf.Ceil(value)));
						float angle = Gauge.Library.RangeConversion(value,minimumValue,maximumValue,minimumAngle + orientation,maximumAngle + orientation);
						if(weight < 1F)
						{
							weight = 1F - weight;
							position.x = linear < 0F ? -linear : linear;
							position.x = Mathf.Clamp(Gauge.Library.RangeConversion(value,minimumValue,maximumValue,-linear,linear),-position.x,position.x) * weight;
							angle = Mathf.Lerp(angle,maximumAngle + minimumAngle + orientation,weight);
						}
						PositionHandler(needle.pivot,position,angle);
						PositionHandler(needle.movables,position - Vector2.up * scale.y * pivotPoint,angle);
						ScaleHandler(needle.scalables,scale);
						ScaleHandler(needle.pivot,Vector2.one * scale.x * pivotScale * Mathf.Sign(scale.y));
						RotationHandler(needle.movables,angle);
						RotationHandler(needle.pivot,angle);
						updateNeedleTransform = false;
					}
					if(updateNeedleColor && overrideColor)
					{
						for(int a = 0,A = needle.colorables.Count; a < A; a++)
						{
							if(component == 0)ColorHandler(needle.colorables[a].spriteRenderer,color);
							else if(component == 1)ColorHandler(needle.colorables[a].image,color);
							else if(component == 2)ColorHandler(needle.colorables[a].rawImage,color);
							else ColorHandler(needle.colorables[a].meshRenderer,material);
						}
						updateNeedleColor = false;
					}
					#if UNITY_EDITOR
				}
				#endif
			}
			public void SetIntegerize (Integerize value) {integerize = value;}
			public void SetIntegerize (int value) {integerize = (Integerize)value;}
			public void SetIndex (sbyte value) {index = value;}
			public void SetMaterial (Material value) {material = value;}
			public void SetPivotPoint (float value) {pivotPoint = value;}
			public void SetPivotOffset (float value) {pivotOffset = value;}
			public void SetPivotOffsetFactor (float value) {pivotOffsetFactor = value;}
			public void SetHeadOffset (float value) {headOffset = value;}
			public void SetHeadOffsetFactor (float value) {headOffsetFactor = value;}
			public void SetScale (Vector2 value) {scale = value;}
			public void SetScale (Vector3 value) {scale = new Vector2(value.x,value.y);}
			public void SetScaleMultiplier (Vector2 value) {scaleMultiplier = value;}
			public void SetScaleMultiplier (Vector3 value) {scaleMultiplier = new Vector2(value.x,value.y);}
			public void SetScaleFactor (float value) {scaleFactor = value;}
			public void SetPivotScale (float value) {pivotScale = value;}
			public void SetOverrideColor (bool value) {overrideColor = value;}
			public void SetColor (Color value) {color = value;}
			public void UpdateNeedleTransform () {updateNeedleTransform = true;}
			public void UpdateNeedleColor () {updateNeedleColor = true;}
		}
		public Dimension dimension = Dimension.TwoDimensional;
		public Type type = Type.Analog;
		public TwoDimensional twoDimensional = TwoDimensional.Image;
		public ThreeDimensional threeDimensional = ThreeDimensional.SpriteRenderer;
		public Digital digital = Digital.Activation;
		public Color ticksColorOn = Color.white;
		public Color ticksColorOff = Color.gray;
		public Material sampleMaterial = null;
		public Material primaryMaterial = null;
		public Material secondaryMaterial = null;
		[Tooltip("-1: Don't update.\n 0: Update on awake.\n N: Update once every N frames.")]
		public sbyte tickTransformUpdates = 10;
		[Tooltip("-1: Don't update.\n 0: Update on awake.\n N: Update once every N frames.")]
		public sbyte tickColorUpdates = 3;
		[Tooltip("-1: Don't update.\n 0: Update on awake.\n N: Update once every N frames.")]
		public sbyte tickTextUpdates = 100;
		[Tooltip("-1: Don't update.\n 0: Update on awake.\n N: Update once every N frames.")]
		public sbyte tickTextColorUpdates = 3;
		public byte minorTicks = 2;
		public List<string> majorTicks = new List<string>();
		public GameObject majorTickPrefab = null;
		public GameObject textPrefab = null;
		public GameObject minorTickPrefab = null;
		public bool connectEndpoints = false;
		public float orientation = 0F;
		[Range(360F,-360F)] public float minimumAngle = 90F;
		[Range(360F,-360F)] public float maximumAngle = -90F;
		[Range(0,100)] public byte circleTolerance = 100;
		public List<Shape> shapes = new List<Shape>();
		public float minimumValue = 0F;
		public Range range = Range.Clamp;
		public float value = 0F;
		public float maximumValue = 100F;
		public bool useNeedle = false;
		[Tooltip("-1: Don't update.\n 0: Update on awake.\n N: Update once every N frames.")]
		public sbyte needleTransformUpdates = 1;
		[Tooltip("-1: Don't update.\n 0: Update on awake.\n N: Update once every N frames.")]
		public sbyte needleColorUpdates = 40;
		public GameObject needlePrefab = null;
		public List<AdditionalValue> additionalValues = new List<AdditionalValue>();
		public List<Needle> needles = new List<Needle>();
		public bool useText = false;
		[Tooltip("-1: Don't update.\n 0: Update on awake.\n N: Update once every N frames.")]
		public sbyte textUpdates = 3;
		[Tooltip("-1: Don't update.\n 0: Update on awake.\n N: Update once every N frames.")]
		public sbyte textTransformUpdates = 100;
		[Tooltip("-1: Don't update.\n 0: Update on awake.\n N: Update once every N frames.")]
		public sbyte textColorUpdates = 40;
		public bool absolute = true;
		public Integerize integerize = Integerize.Round;
		[Tooltip("Index of additional values to use, leave it as -1 if you don't know what it does, -1 means to use the built-in value, minimum value and maximum value.")]
		public sbyte index = -1;
		public GaugeBodyPart.Text text = null;
		public Vector2 textOffset = Vector2.zero;
		public float textOffsetFactor = 1F;
		public Vector2 textScale = Vector2.one * 0.03F;
		public float textScaleFactor = 0.8F;
		public bool overrideTextColor = true;
		public Color textColor = Color.white;
		public float factor = 1F;
		public float radius = 0.5F;
		public float radiusFactor = 1F;
		public float majorTickPivot = 1F;
		public Vector2 majorTickOffset = new Vector2(0F,-0.03F);
		public float majorTickOffsetFactor = 1F;
		public Vector2 majorTickScale = new Vector2(0.03F,0.07F);
		public float majorTickScaleFactor = 1F;
		public bool overrideMajorTickColor = true;
		public Color majorTickColor = Color.white;
		public bool useTickText = true;
		public bool verticalRotation = true;
		public Vector2 tickTextAnchor = new Vector2(0F,-0.5F);
		public Vector2 tickTextOffset = new Vector2(0F,-0.1F);
		public float tickTextOffsetFactor = 1F;
		public Vector2 tickTextScale = Vector2.one * 0.03F;
		public float tickTextScaleFactor = 0.6F;
		public bool overrideTickTextColor = true;
		public Color tickTextColor = Color.white;
		public float minorTickPivot = 1F;
		public Vector2 minorTickOffset = new Vector2(0F,-0.03F);
		public float minorTickOffsetFactor = 1F;
		public Vector2 minorTickScale = new Vector2(0.03F,0.07F);
		public float minorTickScaleAnalogFactor = 0.7F;
		public float minorTickScaleDigitalFactor = 1F;
		public bool overrideMinorTickColor = true;
		public Color minorTickColor = Color.white;
		[HideInInspector] public GaugeBodyPart[] ticks = new GaugeBodyPart[0];
		[HideInInspector] public GaugeMask mask = null;
		[HideInInspector] public bool updateTickTransform = true,updateTickColor = true,updateTickText = true,updateTickTextColor = true,updateNeedleTransform = true,updateNeedleColor = true,updateText = true,updateTextTransform = true,updateTextColor = true;
		[HideInInspector,SerializeField] private GameObject majorTickTracker = null,minorTickTracker = null,textTracker = null;
		#if UNITY_EDITOR
		private void Reset ()
		{
			GaugeBodyPart[] bodies = GetComponentsInChildren<GaugeBodyPart>();
			for(int a = 0,A = bodies.Length; a < A; a++)
			{
				if(bodies[a].gameObject == gameObject)continue;
				UnityEditor.Undo.DestroyObjectImmediate(bodies[a].gameObject);
			}
			if(transform.GetType() == typeof(Transform))
				dimension = Dimension.ThreeDimensional;
		}
		#endif
		private void Awake ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)return;
			#endif
			updateTickTransform = tickTransformUpdates >= 0;
			updateTickColor = tickColorUpdates >= 0;
			updateTickText = tickTextUpdates >= 0;
			updateTickTextColor = tickTextColorUpdates >= 0;
			updateNeedleTransform = needleTransformUpdates >= 0;
			updateNeedleColor = needleColorUpdates >= 0;
			updateText = textUpdates >= 0;
			updateTextTransform = textTransformUpdates >= 0;
			updateTextTransform = textColorUpdates >= 0;
			for(int a = 0,A = needles.Count; a < A; a++)
			{
				Needle needle = needles[a];
				needle.updateNeedleTransform = updateNeedleTransform;
				needle.updateNeedleColor = updateNeedleColor;
				needle.tracker = needlePrefab;
			}
			majorTickTracker = majorTickPrefab;
			minorTickTracker = minorTickPrefab;
			textTracker = textPrefab;
		}
		private void Update ()
		{
			ValidationHandler();
			#if UNITY_EDITOR
			if(!runInEditor && !Application.isPlaying)return;
			#endif
			CheckHandler();
			ExecutionHandler();
		}
		[HideInInspector] private byte component = 0;
		private void ValidationHandler ()
		{
			component = dimension == Dimension.TwoDimensional ? (byte)twoDimensional : (byte)threeDimensional;
			if(minorTicks > 50)minorTicks = 50;
			if(orientation <= -360F)orientation = orientation % -360F;
			else if(orientation >= 360F)orientation = orientation % 360F;
			if(minimumAngle < -360F)minimumAngle = -360F;
			else if(minimumAngle > 360F)minimumAngle = 360F;
			if(maximumAngle < -360F)maximumAngle = -360F;
			else if(maximumAngle > 360F)maximumAngle = 360F;
			if(circleTolerance > 100)circleTolerance = 100;
			for(int a = 0,A = shapes.Count; a < A; a++)shapes[a].Update();
			for(int a = 0,A = additionalValues.Count; a < A; a++)additionalValues[a].Update();
			if(minimumValue < float.MinValue)minimumValue = float.MinValue;
			else if(minimumValue > float.MaxValue)minimumValue = float.MaxValue;
			if(range == Range.None)
			{
				if(value < float.MinValue)value = float.MinValue;
				else if(value > float.MaxValue)value = float.MaxValue;
			}
			else
			{
				float minimum = minimumValue <= maximumValue ? minimumValue : maximumValue;
				float maximum = maximumValue >= minimumValue ? maximumValue : minimumValue;
				if(value < minimum)value = range == Range.Clamp ? minimum : (minimum != maximum ? maximum - (minimum - value) % (maximum - minimum) : minimum);
				else if(value > maximum)value = range == Range.Clamp ? maximum : (minimum != maximum ? minimum - (maximum - value) % (minimum - maximum) : minimum);
			}
			if(maximumValue < float.MinValue)maximumValue = float.MinValue;
			else if(maximumValue > float.MaxValue)maximumValue = float.MaxValue;
			if(factor < 0F)factor = 0F;
			else if(factor > float.MaxValue)factor = float.MaxValue;
			if(radius < 0F)radius = 0F;
			else if(radius > float.MaxValue)radius = float.MaxValue;
			if(radiusFactor < 0F)radiusFactor = 0F;
			else if(radiusFactor > float.MaxValue)radiusFactor = float.MaxValue;
			if(!useNeedle)
			{
				needlePrefab = null;
				#if UNITY_EDITOR
				if(!runInEditor && !Application.isPlaying && needles.Count != 0)
				{
					for(int a = 0,A = needles.Count; a < A; a++)
					{
						Needle needle = needles[a];
						if(!Application.isPlaying)
						{
							if(needle.needle)DestroyImmediate(needle.needle.gameObject);
							if(needle.material)DestroyImmediate(needle.material);
						}
						else
						{
							if(needle.needle)Destroy(needle.needle.gameObject);
							if(needle.material)Destroy(needle.material);
						}
					}
					needles.Clear();
				}
				#endif
			}
			#if UNITY_EDITOR
			else if(!runInEditor && !Application.isPlaying)for(int a = 0,A = needles.Count; a < A; a++)
			{
				Needle needle = needles[a];
				if(needle.index < -1)needle.index = -1;
				else if(additionalValues.Count != 0) {if(needle.index >= additionalValues.Count)needle.index = (sbyte)(additionalValues.Count - 1);}
				else if(needle.index > -1)needle.index = -1;
				needle.Update(component,sampleMaterial);
				needle.tracker = needlePrefab;
			}
			#endif
			if(!useText)
			{
				if(!useTickText)textPrefab = null;
				if(text != null)
				{
					if(text.root)
					{
						#if UNITY_EDITOR
						if(!Application.isPlaying)DestroyImmediate(text.root.gameObject);
						else
						#endif
						Destroy(text.root.gameObject);
					}
					text = null;
				}
			}
			else
			{
				if(index < -1)index = -1;
				else if(additionalValues.Count != 0) {if(index >= additionalValues.Count)index = (sbyte)(additionalValues.Count - 1);}
				else if(index > -1)index = -1;
			}
			if(dimension == Dimension.TwoDimensional)
			{
				if(component == 0 && (majorTickPrefab == resources.tick2DImage || majorTickPrefab == resources.tick2DRawImage) || component == 1 && (majorTickPrefab == resources.tickSpriteRenderer || majorTickPrefab == resources.tick2DRawImage) || component == 2 && (majorTickPrefab == resources.tickSpriteRenderer || majorTickPrefab == resources.tick2DImage) || majorTickPrefab == resources.tick3DMeshRenderer || majorTickPrefab == resources.needleSpriteRenderer || majorTickPrefab == resources.needle2DImage || majorTickPrefab == resources.needle2DRawImage || majorTickPrefab == resources.needle3DMeshRenderer || majorTickPrefab == resources.text2D || majorTickPrefab == resources.text3D || !majorTickPrefab)
					majorTickPrefab = component == 0 ? resources.tickSpriteRenderer : (component == 1 ? resources.tick2DImage : resources.tick2DRawImage);
				if(component == 0 && (minorTickPrefab == resources.tick2DImage || minorTickPrefab == resources.tick2DRawImage) || component == 1 && (minorTickPrefab == resources.tickSpriteRenderer || minorTickPrefab == resources.tick2DRawImage) || component == 2 && (minorTickPrefab == resources.tickSpriteRenderer || minorTickPrefab == resources.tick2DImage) || minorTickPrefab == resources.tick3DMeshRenderer || minorTickPrefab == resources.needleSpriteRenderer || minorTickPrefab == resources.needle2DImage || minorTickPrefab == resources.needle2DRawImage || minorTickPrefab == resources.needle3DMeshRenderer || minorTickPrefab == resources.text2D || minorTickPrefab == resources.text3D || !minorTickPrefab)
					minorTickPrefab = component == 0 ? resources.tickSpriteRenderer : (component == 1 ? resources.tick2DImage : resources.tick2DRawImage);
				if(useNeedle && (needlePrefab == resources.tickSpriteRenderer || needlePrefab == resources.tick2DImage || needlePrefab == resources.tick2DRawImage || needlePrefab == resources.tick3DMeshRenderer || component == 0 && (needlePrefab == resources.needle2DImage || needlePrefab == resources.needle2DRawImage) || component == 1 && (needlePrefab == resources.needleSpriteRenderer || needlePrefab == resources.needle2DRawImage) || component == 2 && (needlePrefab == resources.needleSpriteRenderer || needlePrefab == resources.needle2DImage) || needlePrefab == resources.needle3DMeshRenderer || needlePrefab == resources.text2D || needlePrefab == resources.text3D || !needlePrefab))
					needlePrefab = component == 0 ? resources.needleSpriteRenderer : (component == 1 ? resources.needle2DImage : resources.needle2DRawImage);
				if((useText || useTickText) && (textPrefab == resources.tickSpriteRenderer || textPrefab == resources.tick2DImage || textPrefab == resources.tick2DRawImage || textPrefab == resources.tick3DMeshRenderer || textPrefab == resources.needleSpriteRenderer || textPrefab == resources.needle2DImage || textPrefab == resources.needle2DRawImage || textPrefab == resources.needle3DMeshRenderer || component == 0 && textPrefab == resources.text2D || component != 0 && textPrefab == resources.text3D || !textPrefab))
					textPrefab = component == 0 ? resources.text3D : resources.text2D;
				if(primaryMaterial)
				{
					#if UNITY_EDITOR
					if(!Application.isPlaying)DestroyImmediate(primaryMaterial);
					else
					#endif
					Destroy(primaryMaterial);
				}
				if(secondaryMaterial)
				{
					#if UNITY_EDITOR
					if(!Application.isPlaying)DestroyImmediate(secondaryMaterial);
					else
					#endif
					Destroy(secondaryMaterial);
				}
			}
			else
			{
				if(component == 0 && majorTickPrefab == resources.tick3DMeshRenderer || component == 3 && majorTickPrefab == resources.tickSpriteRenderer || majorTickPrefab == resources.tick2DImage || majorTickPrefab == resources.tick2DRawImage || majorTickPrefab == resources.needleSpriteRenderer || majorTickPrefab == resources.needle2DImage || majorTickPrefab == resources.needle2DRawImage || majorTickPrefab == resources.needle3DMeshRenderer || majorTickPrefab == resources.text2D || majorTickPrefab == resources.text3D || !majorTickPrefab)
					majorTickPrefab = component == 0 ? resources.tickSpriteRenderer : resources.tick3DMeshRenderer;
				if(component == 0 && minorTickPrefab == resources.tick3DMeshRenderer || component == 3 && minorTickPrefab == resources.tickSpriteRenderer || minorTickPrefab == resources.tick2DImage || minorTickPrefab == resources.tick2DRawImage || minorTickPrefab == resources.needleSpriteRenderer || minorTickPrefab == resources.needle2DImage || minorTickPrefab == resources.needle2DRawImage || minorTickPrefab == resources.needle3DMeshRenderer || minorTickPrefab == resources.text2D || minorTickPrefab == resources.text3D || !minorTickPrefab)
					minorTickPrefab = component == 0 ? resources.tickSpriteRenderer : resources.tick3DMeshRenderer;
				if(useNeedle && (needlePrefab == resources.tickSpriteRenderer || needlePrefab == resources.tick2DImage || needlePrefab == resources.tick2DRawImage || needlePrefab == resources.tick3DMeshRenderer || component == 0 && needlePrefab == resources.needle3DMeshRenderer || component == 3 && needlePrefab == resources.needleSpriteRenderer || needlePrefab == resources.needle2DImage || needlePrefab == resources.needle2DRawImage || needlePrefab == resources.text2D || needlePrefab == resources.text3D || !needlePrefab))
					needlePrefab = component == 0 ? resources.needleSpriteRenderer : resources.needle3DMeshRenderer;
				if((useText || useTickText) && (textPrefab == resources.tickSpriteRenderer || textPrefab == resources.tick2DImage || textPrefab == resources.tick2DRawImage || textPrefab == resources.tick3DMeshRenderer || textPrefab == resources.needleSpriteRenderer || textPrefab == resources.needle2DImage || textPrefab == resources.needle2DRawImage || textPrefab == resources.needle3DMeshRenderer || textPrefab == resources.text2D || !textPrefab))
					textPrefab = resources.text3D;
				if(component == 3)
				{
					if(!primaryMaterial)
					{
						if(sampleMaterial)primaryMaterial = new Material(sampleMaterial);
						else
						{
							primaryMaterial = new Material(Shader.Find("Standard"));
							primaryMaterial.SetFloat("_Glossiness",0F);
							primaryMaterial.SetFloat("_Metallic",0F);
						}
						primaryMaterial.name = "Primary";
					}
					if(!secondaryMaterial)
					{
						if(sampleMaterial)secondaryMaterial = new Material(sampleMaterial);
						else
						{
							secondaryMaterial = new Material(Shader.Find("Standard"));
							secondaryMaterial.SetFloat("_Glossiness",0F);
							secondaryMaterial.SetFloat("_Metallic",0F);
						}
						secondaryMaterial.name = "Secondary";
					}
				}
				else
				{
					if(primaryMaterial)
					{
						#if UNITY_EDITOR
						if(!Application.isPlaying)DestroyImmediate(primaryMaterial);
						else
						#endif
						Destroy(primaryMaterial);
					}
					if(secondaryMaterial)
					{
						#if UNITY_EDITOR
						if(!Application.isPlaying)DestroyImmediate(secondaryMaterial);
						else
						#endif
						Destroy(secondaryMaterial);
					}
				}
			}
			if(mask && mask.gauge != this)mask = null;
			#if UNITY_EDITOR
			if(from < float.MinValue)from = float.MinValue;
			else if(from > float.MaxValue)from = float.MaxValue;
			if(to < float.MinValue)to = float.MinValue;
			else if(to > float.MaxValue)to = float.MaxValue;
			if(count < 2)count = 2;
			if(!runInEditor && !Application.isPlaying)
			{
				majorTickTracker = majorTickPrefab;
				minorTickTracker = minorTickPrefab;
				textTracker = textPrefab;
			}
			#endif
		}
		public sbyte tickTransformFrames = 0;
		public sbyte tickColorFrames = 0;
		public sbyte tickTextFrames = 0;
		public sbyte tickTextColorFrames = 0;
		public sbyte needleTransformFrames = 0;
		public sbyte needleColorFrames = 0;
		public sbyte textFrames = 0;
		public sbyte textTransformFrames = 0;
		public sbyte textColorFrames = 0;
		private void CheckHandler ()
		{
			byte reset = 0;
			int count = majorTicks.Count;
			if(count > 0)
			{
				count = count * minorTicks + (count - minorTicks);
				if(connectEndpoints)count = count + minorTicks;
			}
			#if UNITY_EDITOR
			if(Application.isPlaying)
			{
				#endif
				if(tickTransformUpdates > 0)
				{
					tickTransformFrames++;
					if(tickTransformFrames >= tickTransformUpdates)
					{
						updateTickTransform = true;
						tickTransformFrames = 0;
					}
				}
				if(tickColorUpdates > 0)
				{
					tickColorFrames++;
					if(tickColorFrames >= tickColorUpdates)
					{
						updateTickColor = true;
						tickColorFrames = 0;
					}
				}
				if(useTickText)
				{
					if(tickTextUpdates > 0)
					{
						tickTextFrames++;
						if(tickTextFrames >= tickTextUpdates)
						{
							updateTickText = true;
							tickTextFrames = 0;
						}
					}
					if(tickTextColorUpdates > 0)
					{
						tickTextColorFrames++;
						if(tickTextColorFrames >= tickTextColorUpdates)
						{
							updateTickTextColor = true;
							tickTextColorFrames = 0;
						}
					}
				}
				if(useNeedle)
				{
					if(needleTransformUpdates > 0)
					{
						needleTransformFrames++;
						if(needleTransformFrames >= needleTransformUpdates)
						{
							updateNeedleTransform = true;
							needleTransformFrames = 0;
						}
					}
					if(needleColorUpdates > 0)
					{
						needleColorFrames++;
						if(needleColorFrames >= needleColorUpdates)
						{
							updateNeedleColor = true;
							needleColorFrames = 0;
						}
					}
				}
				if(useText)
				{
					if(textUpdates > 0)
					{
						textFrames++;
						if(textFrames >= textUpdates)
						{
							updateText = true;
							textFrames = 0;
						}
					}
					if(textTransformUpdates > 0)
					{
						textTransformFrames++;
						if(textTransformFrames >= textTransformUpdates)
						{
							updateTextTransform = true;
							textTransformFrames = 0;
						}
					}
					if(overrideTextColor && textColorUpdates > 0)
					{
						textColorFrames++;
						if(textColorFrames >= textColorUpdates)
						{
							updateTextColor = true;
							textColorFrames = 0;
						}
					}
				}
				#if UNITY_EDITOR
			}
			else
			{
				if(tickTransformUpdates >= 0)updateTickTransform = true;
				if(tickColorUpdates >= 0)updateTickColor = true;
				if(useTickText)
				{
					if(tickTextUpdates >= 0)updateTickText = true;
					if(tickTextColorUpdates >= 0)updateTickTextColor = true;
				}
				if(useNeedle)
				{
					if(needleTransformUpdates >= 0)updateNeedleTransform = true;
					if(needleColorUpdates >= 0)updateNeedleColor = true;
				}
				if(useText)
				{
					if(textUpdates >= 0)updateText = true;
					if(textTransformUpdates >= 0)updateTextTransform = true;
					if(overrideTextColor && textColorUpdates >= 0)updateTextColor = true;
				}
			}
			#endif
			if(tickTransformUpdates >= 0 || tickColorUpdates >= 0)
			{
				if(ticks.Length != count)reset = 1;
				if(majorTickTracker != majorTickPrefab)
				{
					reset = 1;
					majorTickTracker = majorTickPrefab;
				}
				if(minorTickTracker != minorTickPrefab)
				{
					reset = 1;
					minorTickTracker = minorTickPrefab;
				}
			}
			if((tickTextUpdates >= 0 || tickTextColorUpdates >= 0 || textUpdates >= 0 || textTransformUpdates >= 0 || textColorUpdates >= 0) && textTracker != textPrefab)
			{
				if(text != null)
				{
					if(text.root)
					{
						#if UNITY_EDITOR
						if(!Application.isPlaying)DestroyImmediate(text.root.gameObject);
						else
						#endif
						Destroy(text.root.gameObject);
					}
					text = null;
				}
				if(reset == 0)reset = 2;
				textTracker = textPrefab;
			}
			if(reset != 0)
			{
				for(int a = 0,A = ticks.Length; a < A; a++)
				{
					GaugeBodyPart tick = ticks[a];
					if(!tick)continue;
					if(reset != 2)
					{
						#if UNITY_EDITOR
						if(!Application.isPlaying)DestroyImmediate(tick.gameObject);
						else
						#endif
						Destroy(tick.gameObject);
						continue;
					}
					else if(tick.text != null)
					{
						if(tick.text.root)
						{
							#if UNITY_EDITOR
							if(!Application.isPlaying)DestroyImmediate(tick.text.root.gameObject);
							else
							#endif
							Destroy(tick.text.root.gameObject);
						}
						tick.text = null;
					}
				}
				if(reset != 2)ticks = new GaugeBodyPart[count];
			}
		}
		[HideInInspector] private float weight = -1F;
		[HideInInspector] private float currentFactor = 0F;
		[HideInInspector] private float currentTextFactor = 0F;
		[HideInInspector] private float currentRadius = 0F;
		[HideInInspector] private Vector2 currentMajorTickOrigin = Vector2.zero;
		[HideInInspector] private Vector2 currentMajorTickOffset = Vector2.zero;
		[HideInInspector] private Vector2 currentMajorTickScale = Vector2.zero;
		[HideInInspector] private Vector2 currentTickTextOrigin = Vector2.zero;
		[HideInInspector] private Vector2 currentTickTextScale = Vector2.zero;
		[HideInInspector] private Vector2 currentMinorTickOrigin = Vector2.zero;
		[HideInInspector] private Vector2 currentMinorTickScale = Vector2.zero;
		[HideInInspector,SerializeField] private float? lastValue = null;
		private void ExecutionHandler ()
		{
			byte stage = 0;
			#if UNITY_EDITOR
			if(runInEditor && !Application.isPlaying || Application.isPlaying)
			{
				#endif
				if(updateTickColor && component == 3)
				{
					if(type == Type.Analog || digital == Digital.Activation)
					{
						if(overrideMajorTickColor)ColorHandler(primaryMaterial,majorTickColor);
						if(overrideMinorTickColor)ColorHandler(secondaryMaterial,minorTickColor);
					}
					else
					{
						ColorHandler(primaryMaterial,ticksColorOn);
						ColorHandler(secondaryMaterial,ticksColorOff);
					}
				}
				for(int a = 0,A = ticks.Length,textIndex = 0; a < A; a++)
				{
					bool isFirst = minorTicks == 0 ? true : a % (minorTicks + 1) == 0;
					GaugeBodyPart tick = ticks[a];
					if(!tick)
					{
						if(isFirst)
						{
							if(majorTickPrefab)
							{
								if(a == 0 && !majorTickPrefab.GetComponent<GaugeBodyPart>())
								{
									#if UNITY_EDITOR
									Debug.LogError("No Gauge Body Part component was found on the major tick prefab",majorTickPrefab);
									#endif
									break;
								}
								tick = Instantiate(majorTickPrefab).GetComponent<GaugeBodyPart>();
								tick.transform.name = "Major Tick";
								tick.transform.SetParent(transform,false);
								tick.transform.SetSiblingIndex(a);
								ticks[a] = tick;
							}
							else
							{
								#if UNITY_EDITOR
								Debug.LogError("No major tick prefab is assigned",this);
								#endif
								break;
							}
						}
						else
						{
							if(minorTickPrefab)
							{
								if(a == 1 && !minorTickPrefab.GetComponent<GaugeBodyPart>())
								{
									#if UNITY_EDITOR
									Debug.LogError("No Gauge Body Part component was found on the minor tick prefab",minorTickPrefab);
									#endif
									break;
								}
								tick = Instantiate(minorTickPrefab).GetComponent<GaugeBodyPart>();
								tick.transform.name = "Minor Tick";
								tick.transform.SetParent(transform,false);
								tick.transform.SetSiblingIndex(a);
								ticks[a] = tick;
							}
							else
							{
								#if UNITY_EDITOR
								Debug.LogError("No minor tick prefab is assigned",this);
								#endif
								break;
							}
						}
						updateTickTransform = true;
						updateTickColor = true;
					}
					if(updateTickTransform)
					{
						float angle = !connectEndpoints ? SegmentAngles(minimumAngle + orientation,maximumAngle + orientation,a,A) : SegmentAngles(orientation,orientation - 360F,a,A + 1);
						if(isFirst)
						{
							if(stage == 0)
							{
								currentFactor = this.factor;
								currentTextFactor = currentFactor;
								if(dimension == Dimension.TwoDimensional && component != 0)
								{
									currentFactor = currentFactor * 100F;
									currentTextFactor = currentTextFactor * 10F;
								}
								currentRadius = this.radius * radiusFactor * currentFactor;
								currentMajorTickOffset = this.majorTickOffset * majorTickOffsetFactor * currentFactor;
								currentMajorTickOffset.y = Mathf.Clamp(currentMajorTickOffset.y,-currentRadius,0F);
								currentMajorTickScale = this.majorTickScale * majorTickScaleFactor * currentFactor;
								currentMajorTickOrigin = new Vector2(currentMajorTickOffset.x,currentRadius + (currentMajorTickOffset.y + currentMajorTickScale.y * (0.5F - majorTickPivot)));
								stage = 1;
							}
							Vector2 shapeOffset = ShapeHandler(ref angle,currentRadius,currentMajorTickOrigin.y,currentMajorTickScale.x,majorTickPivot,a,A);
							PositionHandler(tick.movables,currentMajorTickOrigin + shapeOffset,angle);
							ScaleHandler(tick.scalables,currentMajorTickScale);
							if(useTickText)
							{
								#if ENABLE_UNITY_TEXT
								if(component == 1 || component == 2)
								{
									#endif
									if(tick.text == null || !tick.text.text)
									{
										if(textPrefab)
										{
											GaugeBodyPart instance = null;
											if(a == 0)
											{
												instance = textPrefab.GetComponent<GaugeBodyPart>();
												if(!instance)
												{
													#if UNITY_EDITOR
													Debug.LogError("No Gauge Body Part component was found on the text prefab",textPrefab);
													#endif
													break;
												}
												else if(instance.text == null || !instance.text.text)
												{
													#if UNITY_EDITOR
													Debug.LogError("No text is assigned on the text prefab",textPrefab);
													#endif
													break;
												}
											}
											instance = Instantiate(textPrefab).GetComponent<GaugeBodyPart>();
											tick.text = new GaugeBodyPart.Text(instance.text.text);
											tick.text.root.name = "Text";
											tick.text.root.SetParent(tick.transform,false);
											#if UNITY_EDITOR
											if(!Application.isPlaying)DestroyImmediate(instance);
											else
											#endif
											Destroy(instance);
										}
										else
										{
											#if UNITY_EDITOR
											Debug.LogError("No text prefab is assigned",this);
											#endif
											break;
										}
										updateTickText = true;
										updateTickTextColor = true;
									}
									if(stage == 1)
									{
										currentTickTextOrigin = tickTextOffset * tickTextOffsetFactor * currentFactor;
										currentTickTextScale = this.tickTextScale * tickTextScaleFactor * currentTextFactor;
										currentTickTextOrigin = new Vector2(currentMajorTickOrigin.x + (currentTickTextOrigin.x + Mathf.Abs(currentMajorTickScale.x) * tickTextAnchor.x),currentMajorTickOrigin.y + (currentTickTextOrigin.y + Mathf.Abs(currentMajorTickScale.y) * tickTextAnchor.y));
										stage = 2;
									}
									PositionHandler(tick.text.text.transform,currentTickTextOrigin + shapeOffset,angle);
									RotationHandler(tick.text.text.transform,verticalRotation ? 0F : angle);
									ScaleHandler(tick.text.text.transform,currentTickTextScale);
									if(updateTickText)
									{
										if(updateTickTextColor && overrideTickTextColor)tick.text.Handle(majorTicks[textIndex],tickTextColor);
										else tick.text.Handle(majorTicks[textIndex]);
										textIndex = textIndex + 1;
									}
									else if(updateTickTextColor && overrideTickTextColor)
										tick.text.Handle(tickTextColor);
									#if ENABLE_UNITY_TEXT
								}
								else
								{
									if(tick.text == null || !tick.text.textMesh)
									{
										if(textPrefab)
										{
											GaugeBodyPart instance = null;
											if(a == 0)
											{
												instance = textPrefab.GetComponent<GaugeBodyPart>();
												if(!instance)
												{
													#if UNITY_EDITOR
													Debug.LogError("No Gauge Body Part component was found on the text prefab",textPrefab);
													#endif
													break;
												}
												else if(instance.text == null || !instance.text.textMesh)
												{
													#if UNITY_EDITOR
													Debug.LogError("No text mesh is assigned on the text prefab",textPrefab);
													#endif
													break;
												}
											}
											instance = Instantiate(textPrefab).GetComponent<GaugeBodyPart>();
											tick.text = new GaugeBodyPart.Text(instance.text.textMesh);
											tick.text.root.name = "Text";
											tick.text.root.SetParent(tick.transform,false);
											#if UNITY_EDITOR
											if(!Application.isPlaying)DestroyImmediate(instance);
											else
											#endif
											Destroy(instance);
										}
										else
										{
											#if UNITY_EDITOR
											Debug.LogError("No text prefab is assigned",this);
											#endif
											break;
										}
										updateTickText = true;
										updateTickTextColor = true;
									}
									if(stage == 1)
									{
										currentTickTextOrigin = tickTextOffset * tickTextOffsetFactor * currentFactor;
										currentTickTextScale = this.tickTextScale * tickTextScaleFactor * currentTextFactor;
										currentTickTextOrigin = new Vector2(currentMajorTickOrigin.x + (currentTickTextOrigin.x + Mathf.Abs(currentMajorTickScale.x) * tickTextAnchor.x),currentMajorTickOrigin.y + (currentTickTextOrigin.y + Mathf.Abs(currentMajorTickScale.y) * tickTextAnchor.y));
										stage = 2;
									}
									PositionHandler(tick.text.textMesh.transform,currentTickTextOrigin + shapeOffset,angle);
									RotationHandler(tick.text.textMesh.transform,verticalRotation ? 0F : angle);
									ScaleHandler(tick.text.textMesh.transform,currentTickTextScale);
									if(updateTickText)
									{
										if(updateTickTextColor && overrideTickTextColor)tick.text.Handle(majorTicks[textIndex],tickTextColor);
										else tick.text.Handle(majorTicks[textIndex]);
										textIndex = textIndex + 1;
									}
									else if(updateTickTextColor && overrideTickTextColor)
										tick.text.Handle(tickTextColor);
								}
								#endif
							}
							else if(tick.text != null)
							{
								if(tick.text.root)
								{
									#if UNITY_EDITOR
									if(!Application.isPlaying)DestroyImmediate(tick.text.root.gameObject);
									else
									#endif
									Destroy(tick.text.root.gameObject);
								}
								tick.text = null;
							}
						}
						else
						{
							if(stage != 3)
							{
								currentMinorTickOrigin = minorTickOffset * minorTickOffsetFactor * currentFactor;
								currentMinorTickScale = this.minorTickScale * (type == Type.Analog ? minorTickScaleAnalogFactor : minorTickScaleDigitalFactor) * currentFactor;
								currentMinorTickOrigin = new Vector2(currentMinorTickOrigin.x,currentRadius + (Mathf.Clamp(currentMinorTickOrigin.y,-currentRadius,0F) + currentMinorTickScale.y * (0.5F - minorTickPivot)));
								stage = 3;
							}
							PositionHandler(tick.movables,currentMinorTickOrigin + ShapeHandler(ref angle,currentRadius,currentMinorTickOrigin.y,currentMinorTickScale.x,minorTickPivot,a,A),angle);
							ScaleHandler(tick.scalables,currentMinorTickScale);
						}
						RotationHandler(tick.movables,angle);
					}
					else if(isFirst)
					{
						if(useTickText)
						{
							#if ENABLE_UNITY_TEXT
							if(component == 1 || component == 2)
							{
								#endif
								if(tick.text == null || !tick.text.text)
								{
									if(textPrefab)
									{
										GaugeBodyPart instance = null;
										if(a == 0)
										{
											instance = textPrefab.GetComponent<GaugeBodyPart>();
											if(!instance)
											{
												#if UNITY_EDITOR
												Debug.LogError("No Gauge Body Part component was found on the text prefab",textPrefab);
												#endif
												break;
											}
											else if(instance.text == null || !instance.text.text)
											{
												#if UNITY_EDITOR
												Debug.LogError("No text is assigned on the text prefab",textPrefab);
												#endif
												break;
											}
										}
										instance = Instantiate(textPrefab).GetComponent<GaugeBodyPart>();
										tick.text = new GaugeBodyPart.Text(instance.text.text);
										tick.text.root.name = "Text";
										tick.text.root.SetParent(tick.transform,false);
										#if UNITY_EDITOR
										if(!Application.isPlaying)DestroyImmediate(instance);
										else
										#endif
										Destroy(instance);
									}
									else
									{
										#if UNITY_EDITOR
										Debug.LogError("No text prefab is assigned",this);
										#endif
										break;
									}
									float angle = !connectEndpoints ? SegmentAngles(minimumAngle + orientation,maximumAngle + orientation,a,A) : SegmentAngles(orientation,orientation - 360F,a,A + 1);
									if(stage == 0)
									{
										currentTickTextOrigin = tickTextOffset * tickTextOffsetFactor * currentFactor;
										currentTickTextScale = this.tickTextScale * tickTextScaleFactor * currentTextFactor;
										currentTickTextOrigin = new Vector2(currentMajorTickOrigin.x + (currentTickTextOrigin.x + Mathf.Abs(currentMajorTickScale.x) * tickTextAnchor.x),currentMajorTickOrigin.y + (currentTickTextOrigin.y + Mathf.Abs(currentMajorTickScale.y) * tickTextAnchor.y));
										stage = 2;
									}
									PositionHandler(tick.text.text.transform,currentTickTextOrigin + ShapeHandler(ref angle,currentRadius,currentMajorTickOrigin.y,currentMajorTickScale.x,majorTickPivot,a,A),angle);
									RotationHandler(tick.text.text.transform,verticalRotation ? 0F : angle);
									ScaleHandler(tick.text.text.transform,currentTickTextScale);
									updateTickText = true;
									updateTickTextColor = true;
								}
								if(updateTickText)
								{
									if(updateTickTextColor && overrideTickTextColor)tick.text.Handle(majorTicks[textIndex],tickTextColor);
									else tick.text.Handle(majorTicks[textIndex]);
									textIndex = textIndex + 1;
								}
								else if(updateTickTextColor && overrideTickTextColor)
									tick.text.Handle(tickTextColor);
								#if ENABLE_UNITY_TEXT
							}
							else
							{
								if(tick.text == null || !tick.text.textMesh)
								{
									if(textPrefab)
									{
										GaugeBodyPart instance = null;
										if(a == 0)
										{
											instance = textPrefab.GetComponent<GaugeBodyPart>();
											if(!instance)
											{
												#if UNITY_EDITOR
												Debug.LogError("No Gauge Body Part component was found on the text prefab",textPrefab);
												#endif
												break;
											}
											else if(instance.text == null || !instance.text.textMesh)
											{
												#if UNITY_EDITOR
												Debug.LogError("No text mesh is assigned on the text prefab",textPrefab);
												#endif
												break;
											}
										}
										instance = Instantiate(textPrefab).GetComponent<GaugeBodyPart>();
										tick.text = new GaugeBodyPart.Text(instance.text.textMesh);
										tick.text.root.name = "Text";
										tick.text.root.SetParent(tick.transform,false);
										#if UNITY_EDITOR
										if(!Application.isPlaying)DestroyImmediate(instance);
										else
										#endif
										Destroy(instance);
									}
									else
									{
										#if UNITY_EDITOR
										Debug.LogError("No text prefab is assigned",this);
										#endif
										break;
									}
									float angle = !connectEndpoints ? SegmentAngles(minimumAngle + orientation,maximumAngle + orientation,a,A) : SegmentAngles(orientation,orientation - 360F,a,A + 1);
									if(stage == 0)
									{
										currentTickTextOrigin = tickTextOffset * tickTextOffsetFactor * currentFactor;
										currentTickTextScale = this.tickTextScale * tickTextScaleFactor * currentTextFactor;
										currentTickTextOrigin = new Vector2(currentMajorTickOrigin.x + (currentTickTextOrigin.x + Mathf.Abs(currentMajorTickScale.x) * tickTextAnchor.x),currentMajorTickOrigin.y + (currentTickTextOrigin.y + Mathf.Abs(currentMajorTickScale.y) * tickTextAnchor.y));
										stage = 2;
									}
									PositionHandler(tick.text.textMesh.transform,currentTickTextOrigin + ShapeHandler(ref angle,currentRadius,currentMajorTickOrigin.y,currentMajorTickScale.x,majorTickPivot,a,A),angle);
									RotationHandler(tick.text.textMesh.transform,verticalRotation ? 0F : angle);
									ScaleHandler(tick.text.textMesh.transform,currentTickTextScale);
									updateTickText = true;
									updateTickTextColor = true;
								}
								if(updateTickText)
								{
									if(updateTickTextColor && overrideTickTextColor)tick.text.Handle(majorTicks[textIndex],tickTextColor);
									else tick.text.Handle(majorTicks[textIndex]);
									textIndex = textIndex + 1;
								}
								else if(updateTickTextColor && overrideTickTextColor)
									tick.text.Handle(tickTextColor);
							}
							#endif
						}
						else if(tick.text != null)
						{
							if(tick.text.root)
							{
								#if UNITY_EDITOR
								if(!Application.isPlaying)DestroyImmediate(tick.text.root.gameObject);
								else
								#endif
								Destroy(tick.text.root.gameObject);
							}
							tick.text = null;
						}
					}
					if(updateTickColor)for(int b = 0,B = tick.colorables.Count; b < B; b++)
					{
						GaugeBodyPart.Colorable colorable = tick.colorables[b];
						if(type == Type.Analog || digital == Digital.Color)
						{
							if(component == 0) {if(colorable.spriteRenderer)colorable.spriteRenderer.gameObject.SetActive(true);}
							else if(component == 1) {if(colorable.image)colorable.image.gameObject.SetActive(true);}
							else if(component == 2) {if(colorable.rawImage)colorable.rawImage.gameObject.SetActive(true);}
							else if(colorable.meshRenderer)colorable.meshRenderer.gameObject.SetActive(true);
						}
						if((type == Type.Analog || digital == Digital.Activation) && (isFirst ? overrideMajorTickColor : overrideMinorTickColor))
						{
							if(component == 0)ColorHandler(colorable.spriteRenderer,isFirst ? majorTickColor : minorTickColor);
							else if(component == 1)ColorHandler(colorable.image,isFirst ? majorTickColor : minorTickColor);
							else if(component == 2)ColorHandler(colorable.rawImage,isFirst ? majorTickColor : minorTickColor);
							else ColorHandler(colorable.meshRenderer,isFirst ? primaryMaterial : secondaryMaterial);
						}
						if(type == Type.Digital)
						{
							float result = (maximumValue - minimumValue) / (A - 1F);
							result = a == 0 ? minimumValue : minimumValue + ((result * 0.5F) + (result * (a - 1F)));
							if(digital == Digital.Activation)
							{
								if(component == 0) {if(colorable.spriteRenderer)colorable.spriteRenderer.gameObject.SetActive(maximumValue >= minimumValue ? value >= result : value <= result);}
								else if(component == 1) {if(colorable.image)colorable.image.gameObject.SetActive(maximumValue >= minimumValue ? value >= result : value <= result);}
								else if(component == 2) {if(colorable.rawImage)colorable.rawImage.gameObject.SetActive(maximumValue >= minimumValue ? value >= result : value <= result);}
								else if(colorable.meshRenderer)colorable.meshRenderer.gameObject.SetActive(maximumValue >= minimumValue ? value >= result : value <= result);
							}
							else
							{
								if(component == 0)ColorHandler(colorable.spriteRenderer,(maximumValue >= minimumValue ? value >= result : value <= result) ? ticksColorOn : ticksColorOff);
								else if(component == 1)ColorHandler(colorable.image,(maximumValue >= minimumValue ? value >= result : value <= result) ? ticksColorOn : ticksColorOff);
								else if(component == 2)ColorHandler(colorable.rawImage,(maximumValue >= minimumValue ? value >= result : value <= result) ? ticksColorOn : ticksColorOff);
								else ColorHandler(colorable.meshRenderer,(maximumValue >= minimumValue ? value >= result : value <= result) ? primaryMaterial : secondaryMaterial);
							}
						}
					}
				}
				if(stage != 0 && (!useNeedle || needles.Count == 0 || !updateNeedleTransform))weight = -1F;
				updateTickTransform = false;
				updateTickColor = false;
				updateTickText = false;
				updateTickTextColor = false;
				#if UNITY_EDITOR
			}
			#endif
			if(needles.Count != 0)
			{
				if(useNeedle)
				{
					if(updateNeedleTransform)
					{
						if(stage == 0)
						{
							weight = circleTolerance;
							if(weight < 100F)for(int a = 0,A = shapes.Count; a < A; a++)
							{
								weight = weight + shapes[a].tolerance;
								if(weight >= 100F)break;
							}
							weight = weight * 0.01F;
							currentFactor = this.factor;
							currentTextFactor = currentFactor;
							if(dimension == Dimension.TwoDimensional && component != 0)
							{
								currentFactor = currentFactor * 100F;
								currentTextFactor = currentTextFactor * 10F;
							}
							currentRadius = this.radius * radiusFactor * currentFactor;
							currentMajorTickOffset = this.majorTickOffset * majorTickOffsetFactor * currentFactor;
							currentMajorTickOffset.y = Mathf.Clamp(currentMajorTickOffset.y,-currentRadius,0F);
							currentMajorTickScale = this.majorTickScale * majorTickScaleFactor * currentFactor;
							stage = 4;
						}
						for(int a = 0,A = needles.Count; a < A; a++)
						{
							Needle needle = needles[a];
							if(needle.index < -1)needle.index = -1;
							else if(additionalValues.Count != 0) {if(needle.index >= additionalValues.Count)needle.index = (sbyte)(additionalValues.Count - 1);}
							else if(needle.index > -1)needle.index = -1;
							needle.updateNeedleTransform = true;
							if(updateNeedleColor)needle.updateNeedleColor = true;
							if(needle.index == -1)needle.Update(
												  #if UNITY_EDITOR
												  runInEditor && !Application.isPlaying || Application.isPlaying,
												  #endif
												  needleTransformUpdates >= 0 || needleColorUpdates >= 0,component,sampleMaterial,orientation,!connectEndpoints ? minimumAngle : 0F,!connectEndpoints ? maximumAngle : -360F,weight,minimumValue,this.value,maximumValue,needlePrefab,currentFactor,currentRadius + currentMajorTickOffset.y + currentMajorTickScale.x * (1F - majorTickPivot),transform);
							else
							{
								AdditionalValue value = additionalValues[needle.index];
								needle.Update(
								#if UNITY_EDITOR
								runInEditor && !Application.isPlaying || Application.isPlaying,
								#endif
								needleTransformUpdates >= 0 || needleColorUpdates >= 0,component,sampleMaterial,orientation,!connectEndpoints ? minimumAngle : 0F,!connectEndpoints ? maximumAngle : -360F,weight,value.minimumValue,value.value,value.maximumValue,needlePrefab,currentFactor,currentRadius + currentMajorTickOffset.y + currentMajorTickScale.x * (1F - majorTickPivot),transform);
							}
						}
						weight = -1F;
						updateNeedleTransform = false;
						updateNeedleColor = false;
					}
					else
					{
						for(int a = 0,A = needles.Count; a < A; a++)
						{
							Needle needle = needles[a];
							if(needle.index < -1)needle.index = -1;
							else if(additionalValues.Count != 0) {if(needle.index >= additionalValues.Count)needle.index = (sbyte)(additionalValues.Count - 1);}
							else if(needle.index > -1)needle.index = -1;
							if(updateNeedleColor)needle.updateNeedleColor = true;
							if(needle.index == -1)needle.Update(
												  #if UNITY_EDITOR
												  runInEditor && !Application.isPlaying || Application.isPlaying,
												  #endif
												  needleTransformUpdates >= 0 || needleColorUpdates >= 0,component,sampleMaterial,orientation,!connectEndpoints ? minimumAngle : 0F,!connectEndpoints ? maximumAngle : -360F,weight,minimumValue,this.value,maximumValue,needlePrefab,currentFactor,currentRadius + currentMajorTickOffset.y + currentMajorTickScale.x * (1F - majorTickPivot),transform);
							else
							{
								AdditionalValue value = additionalValues[needle.index];
								needle.Update(
								#if UNITY_EDITOR
								runInEditor && !Application.isPlaying || Application.isPlaying,
								#endif
								needleTransformUpdates >= 0 || needleColorUpdates >= 0,component,sampleMaterial,orientation,!connectEndpoints ? minimumAngle : 0F,!connectEndpoints ? maximumAngle : -360F,weight,value.minimumValue,value.value,value.maximumValue,needlePrefab,currentFactor,currentRadius + currentMajorTickOffset.y + currentMajorTickScale.x * (1F - majorTickPivot),transform);
							}
						}
						updateNeedleColor = false;
					}
				}
				else
				{
					for(int a = 0,A = needles.Count; a < A; a++)
					{
						Needle needle = needles[a];
						#if UNITY_EDITOR
						if(!Application.isPlaying)
						{
							if(needle.needle)DestroyImmediate(needle.needle.gameObject);
							if(needle.material)DestroyImmediate(needle.material);
						}
						else
						{
						#endif
						if(needle.needle)Destroy(needle.needle.gameObject);
						if(needle.material)Destroy(needle.material);
						#if UNITY_EDITOR
						}
						#endif
					}
					needles.Clear();
				}
			}
			if(useText
			#if UNITY_EDITOR
			&& (runInEditor && !Application.isPlaying || Application.isPlaying)
			#endif
			)
			{
				#if ENABLE_UNITY_TEXT
				if(component == 1 || component == 2)
				{
					#endif
					if(text == null || !text.text)
					{
						if(textPrefab)
						{
							GaugeBodyPart instance = textPrefab.GetComponent<GaugeBodyPart>();
							if(!instance)
							{
								#if UNITY_EDITOR
								Debug.LogError("No Gauge Body Part component was found on the text prefab",textPrefab);
								#endif
								return;
							}
							else if(instance.text == null || !instance.text.text)
							{
								#if UNITY_EDITOR
								Debug.LogError("No text is assigned on the text prefab",textPrefab);
								#endif
								return;
							}
							instance = Instantiate(textPrefab).GetComponent<GaugeBodyPart>();
							text = new GaugeBodyPart.Text(instance.text.text);
							text.root.name = "Text";
							text.root.SetParent(transform,false);
							text.root.SetSiblingIndex(ticks.Length);
							#if UNITY_EDITOR
							if(!Application.isPlaying)DestroyImmediate(instance);
							else
							#endif
							Destroy(instance);
						}
						else
						{
							#if UNITY_EDITOR
							Debug.LogError("No text prefab is assigned",this);
							#endif
							return;
						}
						updateText = true;
						updateTextTransform = true;
						updateTextColor = true;
						lastValue = null;
					}
					if(text == null || !text.text)return;
					if(updateTextTransform)
					{
						if(stage != 4)
						{
							currentFactor = this.factor;
							currentTextFactor = currentFactor;
							if(dimension == Dimension.TwoDimensional && component != 0)
							{
								currentFactor = currentFactor * 100F;
								currentTextFactor = currentTextFactor * 10F;
							}
						}
						PositionHandler(text.text.transform,textOffset * textOffsetFactor * currentFactor);
						ScaleHandler(text.text.transform,textScale * textScaleFactor * currentTextFactor);
						updateTextTransform = false;
					}
					if(updateText)
					{
						float currentValue = index == -1 ? value : additionalValues[index].value;
						currentValue = integerize == Integerize.DontIntegerize ? (absolute && currentValue < 0F ? -currentValue : currentValue) :
									   (integerize == Integerize.Cast ? (int)(absolute && currentValue < 0F ? -currentValue : currentValue) :
									   (integerize == Integerize.Floor ? Mathf.Floor(absolute && currentValue < 0F ? -currentValue : currentValue) :
									   (integerize == Integerize.Round ? Mathf.Round(absolute && currentValue < 0F ? -currentValue : currentValue) : Mathf.Ceil(absolute && currentValue < 0F ? -currentValue : currentValue))));
						if(lastValue != currentValue)
						{
							if(updateTextColor)
							{
								if(overrideTextColor)text.Handle(currentValue.ToString(),textColor);
								else text.Handle(currentValue.ToString());
								updateTextColor = false;
							}
							else text.Handle(currentValue.ToString());
							lastValue = currentValue;
						}
						else if(updateTextColor)
						{
							if(overrideTextColor)text.Handle(textColor);
							updateTextColor = false;
						}
						updateText = false;
					}
					else if(updateTextColor)
					{
						if(overrideTextColor)text.Handle(textColor);
						updateTextColor = false;
					}
					#if ENABLE_UNITY_TEXT
				}
				else
				{
					if(text == null || !text.textMesh)
					{
						if(textPrefab)
						{
							GaugeBodyPart instance = textPrefab.GetComponent<GaugeBodyPart>();
							if(!instance)
							{
								#if UNITY_EDITOR
								Debug.LogError("No Gauge Body Part component was found on the text prefab",textPrefab);
								#endif
								return;
							}
							else if(instance.text == null || !instance.text.textMesh)
							{
								#if UNITY_EDITOR
								Debug.LogError("No text mesh is assigned on the text prefab",textPrefab);
								#endif
								return;
							}
							instance = Instantiate(textPrefab).GetComponent<GaugeBodyPart>();
							text = new GaugeBodyPart.Text(instance.text.textMesh);
							text.root.name = "Text";
							text.root.SetParent(transform,false);
							text.root.SetSiblingIndex(ticks.Length);
							#if UNITY_EDITOR
							if(!Application.isPlaying)DestroyImmediate(instance);
							else
							#endif
							Destroy(instance);
						}
						else
						{
							#if UNITY_EDITOR
							Debug.LogError("No text prefab is assigned",this);
							#endif
							return;
						}
						updateText = true;
						updateTextTransform = true;
						updateTextColor = true;
						lastValue = null;
					}
					if(text == null || !text.textMesh)return;
					if(updateTextTransform)
					{
						if(stage != 4)
						{
							currentFactor = this.factor;
							currentTextFactor = currentFactor;
							if(dimension == Dimension.TwoDimensional && component != 0)
							{
								currentFactor = currentFactor * 100F;
								currentTextFactor = currentTextFactor * 10F;
							}
						}
						PositionHandler(text.textMesh.transform,textOffset * textOffsetFactor * currentFactor);
						ScaleHandler(text.textMesh.transform,textScale * textScaleFactor * currentTextFactor);
						updateTextTransform = false;
					}
					if(updateText)
					{
						float currentValue = index == -1 ? value : additionalValues[index].value;
						currentValue = integerize == Integerize.DontIntegerize ? (absolute && currentValue < 0F ? -currentValue : currentValue) :
									   (integerize == Integerize.Cast ? (int)(absolute && currentValue < 0F ? -currentValue : currentValue) :
									   (integerize == Integerize.Floor ? Mathf.Floor(absolute && currentValue < 0F ? -currentValue : currentValue) :
									   (integerize == Integerize.Round ? Mathf.Round(absolute && currentValue < 0F ? -currentValue : currentValue) : Mathf.Ceil(absolute && currentValue < 0F ? -currentValue : currentValue))));
						if(lastValue != currentValue)
						{
							if(updateTextColor)
							{
								if(overrideTextColor)text.Handle(currentValue.ToString(),textColor);
								else text.Handle(currentValue.ToString());
								updateTextColor = false;
							}
							else text.Handle(currentValue.ToString());
							lastValue = currentValue;
						}
						else if(updateTextColor)
						{
							if(overrideTextColor)text.Handle(textColor);
							updateTextColor = false;
						}
						updateText = false;
					}
					else if(updateTextColor)
					{
						if(overrideTextColor)text.Handle(textColor);
						updateTextColor = false;
					}
				}
				#endif
			}
			else lastValue = null;
		}
		private Vector2 ShapeHandler (ref float angle,float radius,float verticalTickOrigin,float horizontalTickScale,float tickPivot,int index,int count)
		{
			Vector2 offset = Vector2.zero;
			if(weight == -1F)
			{
				weight = circleTolerance;
				for(int a = 0,A = shapes.Count; a < A; a++)
				{
					Shape shape = shapes[a];
					weight = weight + shape.tolerance;
					offset.y = offset.y + shape.Offset(angle - orientation,radius,horizontalTickScale,tickPivot);
				}
				weight = weight * 0.01F;
			}
			else for(int a = 0,A = shapes.Count; a < A; a++)
				offset.y = offset.y + shapes[a].Offset(angle - orientation,radius,horizontalTickScale,tickPivot);
			if(weight < 1F)
			{
				offset.y = offset.y * weight;
				offset.x = 1F - weight;
				offset.y = offset.y - verticalTickOrigin * offset.x;
				angle = Mathf.Lerp(angle,!connectEndpoints ? maximumAngle + minimumAngle + orientation : orientation,offset.x);
				offset.x = Gauge.Library.RangeConversion(count > 1 ? index / (count - 1F) : 0F,0F,1F,-radius - currentMajorTickOffset.y - currentMajorTickScale.x * (0.5F - majorTickPivot),radius + currentMajorTickOffset.y + currentMajorTickScale.x * (0.5F - majorTickPivot)) * offset.x;
			}
			else offset.y = offset.y / weight;
			return offset;
		}
		private GaugeMask.Information MaskHandler (Vector2 offset,Vector2 scale,Color color,int index,int count)
		{
			return null;
		}
		private static void PositionHandler (Transform transform,Vector2 position) {if(!transform)return;transform.localPosition = new Vector3(position.x,position.y,0F);}
		private static void PositionHandler (Transform transform,Vector2 position,float angle) {if(!transform)return;transform.localPosition = Quaternion.AngleAxis(angle,Vector3.forward) * new Vector3(position.x,position.y,0F);}
		private static void RotationHandler (Transform transform,float angle) {if(!transform)return;transform.localRotation = Quaternion.AngleAxis(angle,Vector3.forward);}
		private static void ScaleHandler (Transform transform,Vector2 scale) {if(!transform)return;transform.localScale = new Vector3(scale.x,scale.y,1F);}
		private static void ColorHandler (Image image,Color color) {if(!image)return;image.color = color;}
		private static void ColorHandler (RawImage rawImage,Color color) {if(!rawImage)return;rawImage.color = color;}
		private static void ColorHandler (SpriteRenderer spriteRenderer,Color color) {if(!spriteRenderer)return;spriteRenderer.color = color;}
		private static void ColorHandler (MeshRenderer meshRenderer,Material material) {if(!meshRenderer)return;meshRenderer.sharedMaterial = material;}
		private static void ColorHandler (Material material,Color color) {if(!material)return;material.color = color;}
		private static float SegmentAngles (float minimumAngle,float maximumAngle,int index,int count) {return count > 1 ? minimumAngle + ((maximumAngle - minimumAngle) / (count - 1F) * index) : minimumAngle;}
		public void SetDimension (Dimension value) {dimension = value;}
		public void SetDimension (int value) {dimension = (Dimension)value;}
		public void SetType (Type value) {type = value;}
		public void SetType (int value) {type = (Type)value;}
		public void SetTwoDimensional (TwoDimensional value) {twoDimensional = value;}
		public void SetTwoDimensional (int value) {twoDimensional = (TwoDimensional)value;}
		public void SetThreeDimensional (ThreeDimensional value) {threeDimensional = value;}
		public void SetThreeDimensional (int value) {threeDimensional = (ThreeDimensional)value;}
		public void SetDigital (Digital value) {digital = value;}
		public void SetDigital (int value) {digital = (Digital)value;}
		public void SetTicksColorOn (Color value) {ticksColorOn = value;}
		public void SetTicksColorOff (Color value) {ticksColorOff = value;}
		public void SetSampleMaterial (Material value) {sampleMaterial = value;}
		public void SetPrimaryMaterial (Material value) {primaryMaterial = value;}
		public void SetSecondaryMaterial (Material value) {secondaryMaterial = value;}
		public void SetTickTransformUpdates (sbyte value) {tickTransformUpdates = value;}
		public void SetTickColorUpdates (sbyte value) {tickColorUpdates = value;}
		public void SetTickTextUpdates (sbyte value) {tickTextUpdates = value;}
		public void SetTickTextColorUpdates (sbyte value) {tickTextColorUpdates = value;}
		public void SetMinorTicks (byte value) {minorTicks = value;}
		public void SetMajorTicks (List<string> value) {majorTicks = value;}
		public void SetMajorTicksUnlinked (List<string> value) {int A = value.Count;if(majorTicks.Count != A)majorTicks = new List<string>(new string[A]);for(int a = 0; a < A; a++)majorTicks[a] = value[a];}
		public void SetMajorTicks (string[] value) {majorTicks = new List<string>(value);}
		public void SetMajorTicks (List<float> value) {int A = value.Count;if(majorTicks.Count != A)majorTicks = new List<string>(new string[A]);for(int a = 0; a < A; a++)majorTicks[a] = value[a].ToString();}
		public void SetMajorTicks (float[] value) {int A = value.Length;if(majorTicks.Count != A)majorTicks = new List<string>(new string[A]);for(int a = 0; a < A; a++)majorTicks[a] = value[a].ToString();}
		public void SetMajorTicks (List<int> value) {int A = value.Count;if(majorTicks.Count != A)majorTicks = new List<string>(new string[A]);for(int a = 0; a < A; a++)majorTicks[a] = value[a].ToString();}
		public void SetMajorTicks (int[] value) {int A = value.Length;if(majorTicks.Count != A)majorTicks = new List<string>(new string[A]);for(int a = 0; a < A; a++)majorTicks[a] = value[a].ToString();}
		public void RemoveMajorTickAtIndex (int index) {if(index >= 0 && index < majorTicks.Count)majorTicks.RemoveAt(index);}
		public void RemoveAllMajorTicks () {majorTicks.Clear();}
		public void GenerateEmptyRange (ushort count) {majorTicks = new List<string>(new string[count]);}
		public void GenerateRangeAsFloat (ushort count) {GenerateRange(minimumValue,maximumValue,count,Integerize.DontIntegerize);}
		public void GenerateRangeAsFloat (byte index,ushort count) {if(index < additionalValues.Count)GenerateRange(additionalValues[index].minimumValue,additionalValues[index].maximumValue,count,Integerize.DontIntegerize);}
		public void GenerateRangeAsFloat (float from,float to,ushort count) {GenerateRange(from,to,count,Integerize.DontIntegerize);}
		public void GenerateRangeAsInteger (ushort count) {GenerateRange(minimumValue,maximumValue,count,Integerize.Cast);}
		public void GenerateRangeAsInteger (byte index,ushort count) {if(index < additionalValues.Count)GenerateRange(additionalValues[index].minimumValue,additionalValues[index].maximumValue,count,Integerize.Cast);}
		public void GenerateRangeAsInteger (float from,float to,ushort count) {GenerateRange(from,to,count,Integerize.Cast);}
		public void GenerateRange (byte count,Integerize integerize) {GenerateRange(minimumValue,maximumValue,count,integerize);}
		public void GenerateRange (byte index,ushort count,Integerize integerize) {if(index < additionalValues.Count)GenerateRange(additionalValues[index].minimumValue,additionalValues[index].maximumValue,count,integerize);}
		public void GenerateRange (float from,float to,ushort count,Integerize integerize)
		{
			float range = to - from;
			if(majorTicks.Count != count)
				majorTicks = new List<string>(new string[count]);
			for(int a = 0; a < count; a++)
				majorTicks[a] = (integerize == Integerize.DontIntegerize ? Gauge.Library.RangeConversion(range * a / (count - 1F),0F,range,from,to) :
								(integerize == Integerize.Cast ? (int)Gauge.Library.RangeConversion(range * a / (count - 1F),0F,range,from,to) :
								(integerize == Integerize.Floor ? Mathf.Floor(Gauge.Library.RangeConversion(range * a / (count - 1F),0F,range,from,to)) :
								(integerize == Integerize.Round ? Mathf.Round(Gauge.Library.RangeConversion(range * a / (count - 1F),0F,range,from,to)) : Mathf.Ceil(Gauge.Library.RangeConversion(range * a / (count - 1F),0F,range,from,to)))))).ToString();
		}
		public void SetMajorTickPrefab (GameObject value) {majorTickPrefab = value;}
		public void SetMajorTickPrefab (Transform value) {majorTickPrefab = value.gameObject;}
		public void SetTextPrefab (GameObject value) {textPrefab = value;}
		public void SetTextPrefab (Transform value) {textPrefab = value.gameObject;}
		public void SetMinorTickPrefab (GameObject value) {minorTickPrefab = value;}
		public void SetMinorTickPrefab (Transform value) {minorTickPrefab = value.gameObject;}
		public void SetConnectEndpoints (bool value) {connectEndpoints = value;}
		public void SetOrientation (float value) {orientation = value;}
		public void SetMinimumAngle (float value) {minimumAngle = value;}
		public void SetMaximumAngle (float value) {maximumAngle = value;}
		public void SetCircleTolerance (byte value) {circleTolerance = value;}
		public void SetShapes (List<Shape> value) {shapes = value;}
		public void SetShapesUnlinked (List<Shape> value) {int A = value.Count;if(shapes.Count != A)shapes = new List<Shape>(new Shape[A]);for(int a = 0; a < A; a++)shapes[a] = value[a];}
		public void SetShapes (Shape[] value) {shapes = new List<Shape>(value);}
		public void SetMinimumValue (float value) {minimumValue = value;}
		public void SetRange (Range value) {range = value;}
		public void SetRange (int value) {range = (Range)value;}
		public void SetValue (float value) {this.value = value;}
		public void SetMaximumValue (float value) {maximumValue = value;}
		public void UseNeedle (bool value) {useNeedle = value;}
		public void SetNeedleTransformUpdates (sbyte value) {needleTransformUpdates = value;}
		public void SetNeedleColorUpdates (sbyte value) {needleColorUpdates = value;}
		public void SetNeedlePrefab (GameObject value) {needlePrefab = value;}
		public void SetNeedlePrefab (Transform value) {needlePrefab = value.gameObject;}
		public void SetAdditionalValues (List<AdditionalValue> value) {additionalValues = value;}
		public void SetAdditionalValuesUnlinked (List<AdditionalValue> value) {int A = value.Count;if(additionalValues.Count != A)additionalValues = new List<AdditionalValue>(new AdditionalValue[A]);for(int a = 0; a < A; a++)additionalValues[a] = value[a];}
		public void SetAdditionalValues (AdditionalValue[] value) {additionalValues = new List<AdditionalValue>(value);}
		public void SetNeedles (List<Needle> value) {needles = value;}
		public void SetNeedlesUnlinked (List<Needle> value) {int A = value.Count;if(needles.Count != A)needles = new List<Needle>(new Needle[A]);for(int a = 0; a < A; a++)needles[a] = value[a];}
		public void SetNeedles (Needle[] value) {needles = new List<Needle>(value);}
		public void DestroyNeedleAtIndex (int index)
		{
			if(index >= 0 && index < needles.Count)
			{
				Needle needle = needles[index];
				#if UNITY_EDITOR
				if(!Application.isPlaying)
				{
					if(needle.needle)DestroyImmediate(needle.needle.gameObject);
					if(needle.material)DestroyImmediate(needle.material);
				}
				else
				{
				#endif
				if(needle.needle)Destroy(needle.needle.gameObject);
				if(needle.material)Destroy(needle.material);
				#if UNITY_EDITOR
				}
				#endif
			}
		}
		public void DestroyAllNeedles ()
		{
			for(int a = 0,A = needles.Count; a < A; a++)
			{
				Needle needle = needles[a];
				#if UNITY_EDITOR
				if(!Application.isPlaying)
				{
					if(needle.needle)DestroyImmediate(needle.needle.gameObject);
					if(needle.material)DestroyImmediate(needle.material);
				}
				else
				{
				#endif
				if(needle.needle)Destroy(needle.needle.gameObject);
				if(needle.material)Destroy(needle.material);
				#if UNITY_EDITOR
				}
				#endif
			}
		}
		public void ClearNeedleAtIndex (int index)
		{
			if(index >= 0 && index < needles.Count)
			{
				Needle needle = needles[index];
				#if UNITY_EDITOR
				if(!Application.isPlaying)
				{
					if(needle.needle)DestroyImmediate(needle.needle.gameObject);
					if(needle.material)DestroyImmediate(needle.material);
				}
				else
				{
				#endif
				if(needle.needle)Destroy(needle.needle.gameObject);
				if(needle.material)Destroy(needle.material);
				#if UNITY_EDITOR
				}
				#endif
				needles.RemoveAt(index);
			}
		}
		public void ClearAllNeedles ()
		{
			for(int a = 0,A = needles.Count; a < A; a++)
			{
				Needle needle = needles[a];
				#if UNITY_EDITOR
				if(!Application.isPlaying)
				{
					if(needle.needle)DestroyImmediate(needle.needle.gameObject);
					if(needle.material)DestroyImmediate(needle.material);
				}
				else
				{
				#endif
				if(needle.needle)Destroy(needle.needle.gameObject);
				if(needle.material)Destroy(needle.material);
				#if UNITY_EDITOR
				}
				#endif
			}
			needles.Clear();
		}
		public void UseText (bool value) {useText = value;}
		public void SetTextUpdates (sbyte value) {textUpdates = value;}
		public void SetTextTransformUpdates (sbyte value) {textTransformUpdates = value;}
		public void SetTextColorUpdates (sbyte value) {textColorUpdates = value;}
		public void SetAbsolute (bool value) {absolute = value;}
		public void SetIntegerize (Integerize value) {integerize = value;}
		public void SetIntegerize (int value) {integerize = (Integerize)value;}
		public void SetIndex (sbyte value) {index = value;}
		public void SetText (
							 #if ENABLE_REFLECTION
							 Component
							 #elif ENABLE_UNITY_TEXT
							 UnityEngine.UI.Text
							 #else
							 TMPro.TMP_Text
							 #endif
							 value) {if(text != null)text.text = value;}
		public void SetText (GaugeBodyPart value)
		{
			if(!value || value.text == null || text == null)return;text.text = value.text.text;
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(value);
			else
			#endif
			Destroy(value);
		}
		#if ENABLE_UNITY_TEXT
		public void SetTextMesh (TextMesh value) {if(text != null)text.textMesh = value;}
		public void SetTextMesh (GaugeBodyPart value)
		{
			if(!value || value.text == null || text == null)return;text.textMesh = value.text.textMesh;
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(value);
			else
			#endif
			Destroy(value);
		}
		#endif
		public void SetTextOffset (Vector2 value) {textOffset = value;}
		public void SetTextOffset (Vector3 value) {textOffset = new Vector2(value.x,value.y);}
		public void SetTextOffsetFactor (float value) {textOffsetFactor = value;}
		public void SetTextScale (Vector2 value) {textScale = value;}
		public void SetTextScale (Vector3 value) {textScale = new Vector2(value.x,value.y);}
		public void SetTextScaleFactor (float value) {textScaleFactor = value;}
		public void SetOverrideTextColor (bool value) {overrideTextColor = value;}
		public void SetTextColor (Color value) {textColor = value;}
		public void SetFactor (float value) {factor = value;}
		public void SetRadius (float value) {radius = value;}
		public void SetRadiusFactor (float value) {radiusFactor = value;}
		public void SetMajorTickPivot (float value) {majorTickPivot = value;}
		public void SetMajorTickOffset (Vector2 value) {majorTickOffset = value;}
		public void SetMajorTickOffset (Vector3 value) {majorTickOffset = new Vector2(value.x,value.y);}
		public void SetMajorTickOffsetFactor (float value) {majorTickOffsetFactor = value;}
		public void SetMajorTickScale (Vector2 value) {majorTickScale = value;}
		public void SetMajorTickScale (Vector3 value) {majorTickScale = new Vector2(value.x,value.y);}
		public void SetMajorTickScaleFactor (float value) {majorTickScaleFactor = value;}
		public void SetOverrideMajorTickColor (bool value) {overrideMajorTickColor = value;}
		public void SetMajorTickColor (Color value) {majorTickColor = value;}
		public void UseTickText (bool value) {useTickText = value;}
		public void SetVerticalRotation (bool value) {verticalRotation = value;}
		public void SetTickTextAnchor (Vector2 value) {tickTextAnchor = value;}
		public void SetTickTextAnchor (Vector3 value) {tickTextAnchor = new Vector2(value.x,value.y);}
		public void SetTickTextOffset (Vector2 value) {tickTextOffset = value;}
		public void SetTickTextOffset (Vector3 value) {tickTextOffset = new Vector2(value.x,value.y);}
		public void SetTickTextOffsetFactor (float value) {tickTextOffsetFactor = value;}
		public void SetTickTextScale (Vector2 value) {tickTextScale = value;}
		public void SetTickTextScale (Vector3 value) {tickTextScale = new Vector2(value.x,value.y);}
		public void SetTickTextScaleFactor (float value) {tickTextScaleFactor = value;}
		public void SetOverrideTickTextColor (bool value) {overrideTickTextColor = value;}
		public void SetTickTextColor (Color value) {tickTextColor = value;}
		public void SetMinorTickPivot (float value) {minorTickPivot = value;}
		public void SetMinorTickOffset (Vector2 value) {minorTickOffset = value;}
		public void SetMinorTickOffset (Vector3 value) {minorTickOffset = new Vector2(value.x,value.y);}
		public void SetMinorTickOffsetFactor (float value) {minorTickOffsetFactor = value;}
		public void SetMinorTickScale (Vector2 value) {minorTickScale = value;}
		public void SetMinorTickScale (Vector3 value) {minorTickScale = new Vector2(value.x,value.y);}
		public void SetMinorTickScaleAnalogFactor (float value) {minorTickScaleAnalogFactor = value;}
		public void SetMinorTickScaleDigitalFactor (float value) {minorTickScaleDigitalFactor = value;}
		public void SetOverrideMinorTickColor (bool value) {overrideMinorTickColor = value;}
		public void SetMinorTickColor (Color value) {minorTickColor = value;}
		public void UpdateAll ()
		{
			updateTickTransform = true;
			updateTickColor = true;
			updateTickText = true;
			updateTickTextColor = true;
			updateNeedleTransform = true;
			updateNeedleColor = true;
			updateText = true;
			updateTextTransform = true;
			updateTextColor = true;
		}
		public void UpdateTickTransform () {updateTickTransform = true;}
		public void UpdateTickColor () {updateTickColor = true;}
		public void UpdateTickText () {updateTickText = true;}
		public void UpdateTickTextColor () {updateTickTextColor = true;}
		public void UpdateNeedleTransform () {updateNeedleTransform = true;}
		public void UpdateNeedleColor () {updateNeedleColor = true;}
		public void UpdateText () {updateText = true;}
		public void UpdateTextTransform () {updateTextTransform = true;}
		public void UpdateTextColor () {updateTextColor = true;}
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
		public void RemoveComponentCleanup ()
		{
			for(int a = 0,A = ticks.Length; a < A; a++)
			{
				GaugeBodyPart tick = ticks[a];
				if(!tick)continue;
				#if UNITY_EDITOR
				if(!Application.isPlaying)DestroyImmediate(tick.gameObject);
				else
				#endif
				Destroy(tick.gameObject);
			}
			for(int a = 0,A = needles.Count; a < A; a++)
			{
				Needle needle = needles[a];
				if(!needle.needle)continue;
				#if UNITY_EDITOR
				if(!Application.isPlaying)DestroyImmediate(needle.needle.gameObject);
				else
				#endif
				Destroy(needle.needle.gameObject);
			}
			if(text != null && text.text)
				#if UNITY_EDITOR
				if(!Application.isPlaying)DestroyImmediate(text.text.gameObject);
				else
				#endif
				Destroy(text.text.gameObject);
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
		#if UNITY_EDITOR
		[HideInInspector] public bool runInEditor = false;
		[HideInInspector] public bool rangeGeneration = true;
		[HideInInspector] public Integerize integerizeRange = Integerize.Cast;
		[HideInInspector] public float from = 0F;
		[HideInInspector] public float to = 100F;
		[HideInInspector] public ushort count = 5;
		[HideInInspector] public string valuesName = "Untitled";
		[HideInInspector] public Vector2 majorTicksScrollView = Vector2.zero;
		#endif
		[HideInInspector] public Gauge.Resources resources = null;
	}
}