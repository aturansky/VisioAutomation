﻿using System;
using IVisio = Microsoft.Office.Interop.Visio;

namespace VisioAutomation.ShapeSheet
{
    public struct ShapeSheetSurface
    {
        public readonly SurfaceTarget Target;

        public ShapeSheetSurface(SurfaceTarget target)
        {
            this.Target = target;
        }

        public ShapeSheetSurface(IVisio.Page page)
        {
            this.Target = new SurfaceTarget(page);
        }

        public ShapeSheetSurface(IVisio.Master master)
        {
            this.Target = new SurfaceTarget(master);
        }

        public ShapeSheetSurface(IVisio.Shape shape)
        {
            this.Target = new SurfaceTarget(shape);
        }

        public int SetFormulas(Streams.StreamArray stream, object[] formulas, short flags)
        {
            if (formulas.Length != stream.Count)
            {
                string msg =
                    string.Format("stream contains {0} items ({1} short values) and requires {2} formula values",
                        stream.Count, stream.Array.Length, stream.Count);
                throw new ArgumentException(msg);
            }

            if (this.Target.Shape != null)
            {
                return this.Target.Shape.SetFormulas(stream.Array, formulas, flags);
            }
            else if (this.Target.Master != null)
            {
                return this.Target.Master.SetFormulas(stream.Array, formulas, flags);
            }
            else if (this.Target.Page != null)
            {
                return this.Target.Page.SetFormulas(stream.Array, formulas, flags);
            }

            throw new System.ArgumentException("Unhandled Target");
        }

        public int SetResults(Streams.StreamArray stream, object[] unitcodes, object[] results, short flags)
        {
            if (results.Length != stream.Count)
            {
                string msg =
                    string.Format("stream contains {0} items ({1} short values) and requires {2} result values",
                        stream.Count, stream.Array.Length, stream.Count);
                throw new ArgumentException(msg);
            }

            if (this.Target.Shape != null)
            {
                return this.Target.Shape.SetResults(stream.Array, unitcodes, results, flags);
            }
            else if (this.Target.Master != null)
            {
                return this.Target.Master.SetResults(stream.Array, unitcodes, results, flags);
            }
            else if (this.Target.Page != null)
            {
                return this.Target.Page.SetResults(stream.Array, unitcodes, results, flags);
            }

            throw new System.ArgumentException("Unhandled Target");
        }

        public TResult[] GetResults<TResult>(Streams.StreamArray stream, object[] unitcodes)
        {
            if (stream.Array.Length == 0)
            {
                return new TResult[0];
            }

            EnforceValidResultType(typeof(TResult));

            var flags = TypeToVisGetSetArgs(typeof(TResult));

            System.Array results_sa = null;

            if (this.Target.Master != null)
            {
                this.Target.Master.GetResults(stream.Array, (short)flags, unitcodes, out results_sa);
            }
            else if (this.Target.Page != null)
            {
                this.Target.Page.GetResults(stream.Array, (short)flags, unitcodes, out results_sa);
            }
            else if (this.Target.Shape != null)
            {
                this.Target.Shape.GetResults(stream.Array, (short)flags, unitcodes, out results_sa);
            }
            else
            {
                throw new System.ArgumentException("Unhandled Target");
            }

            var results = system_array_to_typed_array<TResult>(results_sa);
            return results;
        }

        public string[] GetFormulasU(Streams.StreamArray stream)
        {
            if (stream.Array.Length==0)
            {
                return new string[0];
            }

            System.Array formulas_sa = null;

            if (this.Target.Master != null)
            {
                this.Target.Master.GetFormulasU(stream.Array, out formulas_sa);
            }
            else if (this.Target.Page != null)
            {
                this.Target.Page.GetFormulasU(stream.Array, out formulas_sa);
            }
            else if (this.Target.Shape != null)
            {
                this.Target.Shape.GetFormulasU(stream.Array, out formulas_sa);
            }
            else
            {
                throw new System.ArgumentException("Unhandled Drawing Surface");
            }

            var formulas = system_array_to_typed_array<string>(formulas_sa);
            return formulas;
        }

        private static T[] system_array_to_typed_array<T>(Array results_sa)
        {
            var results = new T[results_sa.Length];
            results_sa.CopyTo(results, 0);
            return results;
        }

        private static void EnforceValidResultType(System.Type result_type)
        {
            if (!IsValidResultType(result_type))
            {
                string msg = string.Format("Unsupported Result Type: {0}", result_type.Name);
                throw new VisioAutomation.Exceptions.InternalAssertionException(msg);
            }
        }

        private static bool IsValidResultType(System.Type result_type)
        {
            return (result_type == typeof(int)
                    || result_type == typeof(double)
                    || result_type == typeof(string));
        }

        private static IVisio.VisGetSetArgs TypeToVisGetSetArgs(System.Type type)
        {
            IVisio.VisGetSetArgs flags;
            if (type == typeof(int))
            {
                flags = IVisio.VisGetSetArgs.visGetTruncatedInts;
            }
            else if (type == typeof(double))
            {
                flags = IVisio.VisGetSetArgs.visGetFloats;
            }
            else if (type == typeof(string))
            {
                flags = IVisio.VisGetSetArgs.visGetStrings;
            }
            else
            {
                string msg = string.Format("Unsupported Result Type: {0}", type.Name);
                throw new VisioAutomation.Exceptions.InternalAssertionException(msg);
            }
            return flags;
        }
    }
}