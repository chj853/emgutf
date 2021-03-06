﻿//----------------------------------------------------------------------------
//  Copyright (C) 2004-2018 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Emgu.TF.Util;

namespace Emgu.TF.Lite
{
    /// <summary>
    /// The tensorflow lite interpreter.
    /// </summary>
    public class Interpreter : Emgu.TF.Util.UnmanagedObject
    {
        /*
        private Interpreter()
        {
            _ptr = TfLiteInvoke.tfeInterpreterCreate();
        }*/

        /// <summary>
        /// Create an interpreter from a flatbuffer model
        /// </summary>
        /// <param name="flatBufferModel">The flat buffer model.</param>
        /// <param name="resolver">An instance that implements the Resolver interface which maps custom op names and builtin op codes to op registrations.</param>
        public Interpreter(FlatBufferModel flatBufferModel, IOpResolver resolver)
        {
            _ptr = TfLiteInvoke.tfeInterpreterCreateFromModel(flatBufferModel.Ptr, resolver.OpResolverPtr);
        }

        /// <summary>
        /// Update allocations for all tensors. This will redim dependent tensors using
        /// the input tensor dimensionality as given. This is relatively expensive.
        /// If you know that your sizes are not changing, you need not call this.
        /// </summary>
        /// <returns>Status of success or failure.</returns>
        public Status AllocateTensors()
        {
            return TfLiteInvoke.tfeInterpreterAllocateTensors(_ptr);
        }

        /// <summary>
        /// Invoke the interpreter (run the whole graph in dependency order).
        /// </summary>
        /// <returns>Status of success or failure.</returns>
        /// <remarks>It is possible that the interpreter is not in a ready state
        /// to evaluate (i.e. if a ResizeTensor() has been performed without an
        /// AllocateTensors().
        /// </remarks>
        public Status Invoke()
        {
            return TfLiteInvoke.tfeInterpreterInvoke(_ptr);
        }

        /*
        public IntPtr GetInputTensorPtr(int index)
        {
            return TfLiteInvoke.tfeInterpreterInputTensor(_ptr, index);
        }

        public IntPtr GetOutputTensorPtr(int index)
        {
            return TfLiteInvoke.tfeInterpreterOuputTensor(_ptr, index);
        }*/

        /// <summary>
        /// Get the number of tensors in the model.
        /// </summary>
        public int TensorSize
        {
            get
            {
                return TfLiteInvoke.tfeInterpreterTensorSize(_ptr);
            }
        }

        /// <summary>
        /// Get the number of ops in the model.
        /// </summary>
        public int NodeSize
        {
            get
            {
                return TfLiteInvoke.tfeInterpreterNodesSize(_ptr);
            }
        }

        /// <summary>
        /// Get a tensor data structure.
        /// </summary>
        /// <param name="index">The index of the tensor</param>
        /// <returns>The tensor in the specific index</returns>
        public Tensor GetTensor(int index)
        {
            return new Tensor(TfLiteInvoke.tfeInterpreterGetTensor(_ptr, index), false);
        }

        /// <summary>
        /// Get the list of tensor index of the inputs tensors.
        /// </summary>
        /// <returns>The list of tensor index of the inputs tensors.</returns>
        public int[] GetInput()
        {
            int size = TfLiteInvoke.tfeInterpreterGetInputSize(_ptr);
            int[] input = new int[size];
            GCHandle handle = GCHandle.Alloc(input, GCHandleType.Pinned);
            TfLiteInvoke.tfeInterpreterGetInput(_ptr, handle.AddrOfPinnedObject());
            handle.Free();
            return input;
        }

        /// <summary>
        /// Get the list of tensor index of the outputs tensors.
        /// </summary>
        /// <returns>The list of tensor index of the outputs tensors.</returns>
        public int[] GetOutput()
        {
            int size = TfLiteInvoke.tfeInterpreterGetOutputSize(_ptr);
            int[] output = new int[size];
            GCHandle handle = GCHandle.Alloc(output, GCHandleType.Pinned);
            TfLiteInvoke.tfeInterpreterGetOutput(_ptr, handle.AddrOfPinnedObject());
            handle.Free();
            return output;
        }

        /// <summary>
        /// Return the name of a given input
        /// </summary>
        /// <param name="index">The input tensor index</param>
        /// <returns>The name of the input tesnsor at the index</returns>
        public String GetInputName(int index)
        {
            IntPtr namePtr = TfLiteInvoke.tfeInterpreterGetInputName(_ptr, index);
            return Marshal.PtrToStringAnsi(namePtr);
        }

        /// <summary>
        /// Return the name of a given output
        /// </summary>
        /// <param name="index">The output tensor index</param>
        /// <returns>The name of the output tesnsor at the index</returns>
        public String GetOutputName(int index)
        {
            IntPtr namePtr = TfLiteInvoke.tfeInterpreterGetOutputName(_ptr, index);
            return Marshal.PtrToStringAnsi(namePtr);
        }

        /// <summary>
        /// Release all the unmanaged memory associated with this interpreter
        /// </summary>
        protected override void DisposeObject()
        {
            if (IntPtr.Zero != _ptr)
                TfLiteInvoke.tfeInterpreterRelease(ref _ptr);
        }
    }

    public static partial class TfLiteInvoke
    {
        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterCreate();

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterCreateFromModel(IntPtr model, IntPtr opResolver);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern Status tfeInterpreterAllocateTensors(IntPtr interpreter);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern Status tfeInterpreterInvoke(IntPtr interpreter);

        /*
        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterInputTensor(IntPtr interpreter, int index);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterOuputTensor(IntPtr interpreter, int index);
        */

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterGetTensor(IntPtr interpreter, int index);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern int tfeInterpreterTensorSize(IntPtr interpreter);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern int tfeInterpreterNodesSize(IntPtr interpreter);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern int tfeInterpreterGetInputSize(IntPtr interpreter);
        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern void tfeInterpreterGetInput(IntPtr interpreter, IntPtr input);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterGetInputName(IntPtr interpreter, int index);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern int tfeInterpreterGetOutputSize(IntPtr interpreter);
        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern void tfeInterpreterGetOutput(IntPtr interpreter, IntPtr output);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern IntPtr tfeInterpreterGetOutputName(IntPtr interpreter, int index);

        [DllImport(ExternLibrary, CallingConvention = TfLiteInvoke.TFCallingConvention)]
        internal static extern void tfeInterpreterRelease(ref IntPtr interpreter);


    }
}
