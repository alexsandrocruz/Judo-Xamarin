using System;
using JudoPayDotNet.Errors;
using System.Collections.Generic;

namespace JudoDotNetXamarin
{
    public static class ErrorExtensions
    {
        public static void FlattenToJudoFailure (this AggregateException errors, JudoFailureCallback failureCallback)
        {
            if (failureCallback != null) {
                List<JudoModelError> models = new List<JudoModelError> (); 
                foreach (Exception em in errors.InnerExceptions) {
                    models.Add (new JudoModelError (){ ErrorMessage = em.Message });
                }
                var judoError = new JudoError () {
                    Exception = errors.InnerException,
                    ApiError = new JudoApiErrorModel (){ ModelErrors = models }
                };

                failureCallback (judoError);

            }
        }

        public static JudoError FlattenToJudoError (this AggregateException errors)
        {
          
            List<JudoModelError> models = new List<JudoModelError> (); 
            foreach (Exception em in errors.InnerExceptions) {
                models.Add (new JudoModelError (){ ErrorMessage = em.Message });
            }
            var judoError = new JudoError () {
                Exception = errors.InnerException,
                ApiError = new JudoApiErrorModel (){ ModelErrors = models }
            };

            return judoError;

        }
    }
}

