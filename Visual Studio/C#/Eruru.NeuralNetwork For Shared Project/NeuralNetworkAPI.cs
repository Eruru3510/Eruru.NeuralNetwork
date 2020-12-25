using System;
using System.Runtime.InteropServices;
using System.Text;
using HDF5DotNet;

namespace Eruru.NeuralNetwork {

	static class NeuralNetworkAPI {

		static readonly Encoding GBK = Encoding.GetEncoding ("GBK");

		public static float Max (float[] values) {
			float max = values[0];
			for (int i = 1; i < values.Length; i++) {
				if (max < values[i]) {
					max = values[i];
				}
			}
			return max;
		}

		public static int IndexOfMax (float[] values) {
			float max = values[0];
			int index = 0;
			for (int i = 0; i < values.Length; i++) {
				if (max < values[i]) {
					max = values[i];
					index = i;
				}
			}
			return index;
		}

		public static float ActivationFunction (NeuralNetworkActivationFunctionType type, float value) {
			switch (type) {
				case NeuralNetworkActivationFunctionType.ReLU:
					return value < 0 ? 0 : value;
				case NeuralNetworkActivationFunctionType.Softmax:
					return value;
				default:
					throw new NotImplementedException (type.ToString ());
			}
		}

		public static int CalculateOutputLength (int inputLength, int size, int stride, NeuralNetworkPaddingType paddingType) {
			switch (paddingType) {
				case NeuralNetworkPaddingType.Valid:
					return (int)Math.Ceiling ((float)(inputLength - size + 1) / stride);
				case NeuralNetworkPaddingType.Same:
					return (int)Math.Ceiling ((float)inputLength / stride);
				default:
					throw new NotImplementedException (paddingType.ToString ());
			}
		}

		public static string Shape (Array array) {
			StringBuilder stringBuilder = new StringBuilder ();
			for (int i = 0; i < array.Length; i++) {
				if (i > 0) {
					stringBuilder.Append ('*');
				}
				stringBuilder.Append (array.GetValue (i));
			}
			return stringBuilder.ToString ();
		}

		public static string PadRight (object value) {
			string text = value.ToString ();
			int amount = GBK.GetByteCount (text) - text.Length;
			return text.PadRight (15 - amount);
		}

		public static NeuralNetworkActivationFunctionType GetActivationFunctionType (string name) {
			switch (name) {
				case "relu":
					return NeuralNetworkActivationFunctionType.ReLU;
				case "softmax":
					return NeuralNetworkActivationFunctionType.Softmax;
				default:
					throw new NotImplementedException (name);
			}
		}

		public static NeuralNetworkDataFormatType GetDataFormatType (string name) {
			switch (name) {
				case "channels_last":
					return NeuralNetworkDataFormatType.ChannelsLast;
				default:
					throw new NotImplementedException (name);
			}
		}

		public static NeuralNetworkPaddingType GetPaddingType (string name) {
			switch (name) {
				case "valid":
					return NeuralNetworkPaddingType.Valid;
				case "same":
					return NeuralNetworkPaddingType.Same;
				default:
					throw new NotImplementedException (name);
			}
		}

		public static string GetAttributeValue (H5ObjectWithAttributes objectWithAttributes, string name) {
			if (objectWithAttributes is null) {
				throw new ArgumentNullException (nameof (objectWithAttributes));
			}
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			H5AttributeId h5AttributeId = H5A.open (objectWithAttributes, name);
			H5DataTypeId h5DataTypeId = H5A.getType (h5AttributeId);
			if (H5T.isVariableString (h5DataTypeId)) {
				VariableLengthString[] variableLengthStrings = new VariableLengthString[1];
				H5A.read (h5AttributeId, h5DataTypeId, new H5Array<VariableLengthString> (variableLengthStrings));
				H5T.close (h5DataTypeId);
				H5A.close (h5AttributeId);
				return variableLengthStrings[0].ToString ();
			}
			byte[] bytes = new byte[H5T.getSize (h5DataTypeId)];
			H5A.read (h5AttributeId, h5DataTypeId, new H5Array<byte> (bytes));
			H5T.close (h5DataTypeId);
			H5A.close (h5AttributeId);
			return Encoding.ASCII.GetString (bytes);
		}

		public static void GetDataSet<T> (H5FileOrGroupId groupOrFileId, string name, out T[] array, int length) {
			if (groupOrFileId is null) {
				throw new ArgumentNullException (nameof (groupOrFileId));
			}
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			H5DataSetId h5DataSetId = H5D.open (groupOrFileId, name);
			H5DataTypeId h5DataTypeId = H5D.getType (h5DataSetId);
			array = new T[length];
			H5D.read (h5DataSetId, h5DataTypeId, new H5Array<T> (array));
			H5T.close (h5DataTypeId);
			H5D.close (h5DataSetId);
		}
		public static void GetDataSet<T> (H5FileOrGroupId groupOrFileId, string name, out T[,] array, int width, int height) {
			if (groupOrFileId is null) {
				throw new ArgumentNullException (nameof (groupOrFileId));
			}
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			H5DataSetId h5DataSetId = H5D.open (groupOrFileId, name);
			H5DataTypeId h5DataTypeId = H5D.getType (h5DataSetId);
			array = new T[height, width];
			H5D.read (h5DataSetId, h5DataTypeId, new H5Array<T> (array));
			H5T.close (h5DataTypeId);
			H5D.close (h5DataSetId);
		}
		public static void GetDataSet<T> (H5FileOrGroupId groupOrFileId, string name, out T[,,,] array, int width, int height, int depth, int number) {
			if (groupOrFileId is null) {
				throw new ArgumentNullException (nameof (groupOrFileId));
			}
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			H5DataSetId h5DataSetId = H5D.open (groupOrFileId, name);
			H5DataTypeId h5DataTypeId = H5D.getType (h5DataSetId);
			array = new T[height, width, depth, number];
			H5D.read (h5DataSetId, h5DataTypeId, new H5Array<T> (array));
			H5T.close (h5DataTypeId);
			H5D.close (h5DataSetId);
		}

		unsafe struct VariableLengthString {

			public char* RecordedText { get; set; }

			public override string ToString () {
				return Marshal.PtrToStringAnsi ((IntPtr)RecordedText);
			}

		}

	}

}