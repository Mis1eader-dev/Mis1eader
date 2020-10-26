namespace Mis1eader.Currency
{
	using UnityEngine;
	using System.Collections.Generic;
	[AddComponentMenu("Mis1eader/Currency/Currency System",0),ExecuteInEditMode]
	public class CurrencySystem : MonoBehaviour
	{
		[System.Serializable] public class Currency
		{
			public string name = string.Empty;
			public double currency = 0D;
			public double maximumCurrency = Mathf.Infinity;
			public void Update ()
			{
				if(maximumCurrency < 0D)maximumCurrency = 0D;
				if(currency < 0D)currency = 0D;
				else if(currency > maximumCurrency)currency = maximumCurrency;
			}
			public void SetName (string value) {name = value;}
			public void SetCurrency (double value) {currency = value;}
			public void DecreaseCurrency (double value) {currency = currency - (value < 0D ? -value : value);}
			public void DecreaseCurrencyByDeltaTime (double value) {currency = currency - (value < 0D ? -value : value) * UnityEngine.Time.deltaTime;}
			public void IncreaseCurrency (double value) {currency = currency + (value < 0D ? -value : value);}
			public void IncreaseCurrencyByDeltaTime (double value) {currency = currency + (value < 0D ? -value : value) * UnityEngine.Time.deltaTime;}
			public void SetMaximumCurrency (double value) {maximumCurrency = value;}
			public void DecreaseMaximumCurrency (double value) {maximumCurrency = maximumCurrency - (value < 0D ? -value : value);}
			public void IncreaseMaximumCurrency (double value) {maximumCurrency = maximumCurrency + (value < 0D ? -value : value);}
		}
		public List<Currency> currencies = new List<Currency>();
		private void Update () {for(int a = 0,A = currencies.Count; a < A; a++)currencies[a].Update();}
		public void SetCurrencies (List<Currency> value) {currencies = value;}
		public void SetCurrenciesUnlinked (List<Currency> value) {int A = value.Count;if(currencies.Count != A)currencies = new List<Currency>(new Currency[A]);for(int a = 0; a < A; a++)currencies[a] = value[a];}
		public void SetCurrencies (Currency[] value) {currencies = new List<Currency>(value);}
		[System.NonSerialized] private int currenciesPointer = 0;
		public void SetCurrenciesPointer (int value) {currenciesPointer = Mathf.Clamp(value,0,currencies.Count - 1);}
		public void SetCurrenciesPointerCurrency (double value) {if(currenciesPointer >= 0 && currenciesPointer < currencies.Count)currencies[currenciesPointer].SetCurrency(value);}
		public void DecreaseCurrenciesPointerCurrency (double value) {if(currenciesPointer >= 0 && currenciesPointer < currencies.Count)currencies[currenciesPointer].DecreaseCurrency(value);}
		public void DecreaseCurrenciesPointerCurrencyByDeltaTime (double value) {if(currenciesPointer >= 0 && currenciesPointer < currencies.Count)currencies[currenciesPointer].DecreaseCurrencyByDeltaTime(value);}
		public void IncreaseCurrenciesPointerCurrency (double value) {if(currenciesPointer >= 0 && currenciesPointer < currencies.Count)currencies[currenciesPointer].IncreaseCurrency(value);}
		public void IncreaseCurrenciesPointerCurrencyByDeltaTime (double value) {if(currenciesPointer >= 0 && currenciesPointer < currencies.Count)currencies[currenciesPointer].IncreaseCurrencyByDeltaTime(value);}
		public void SetCurrenciesPointerMaximumCurrency (double value) {if(currenciesPointer >= 0 && currenciesPointer < currencies.Count)currencies[currenciesPointer].SetMaximumCurrency(value);}
		public void DecreaseCurrenciesPointerMaximumCurrency (double value) {if(currenciesPointer >= 0 && currenciesPointer < currencies.Count)currencies[currenciesPointer].DecreaseMaximumCurrency(value);}
		public void IncreaseCurrenciesPointerMaximumCurrency (double value) {if(currenciesPointer >= 0 && currenciesPointer < currencies.Count)currencies[currenciesPointer].IncreaseMaximumCurrency(value);}
		public void RemoveComponent ()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)DestroyImmediate(this);
			else
			#endif
			Destroy(this);
		}
		#if UNITY_EDITOR
		[HideInInspector] public string currenciesName = "Untitled";
		#endif
	}
}