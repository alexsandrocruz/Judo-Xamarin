using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace JudoDotNetXamarinSDK.Utils
{
    public class CompatibilityAnimation
    {
        private object anim;

        public Action AnimationStart;
        public Action AnimatioEnd;

        public CompatibilityAnimation(Context context, int objectAnimation, int backupAnimation)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                anim = AnimatorInflater.LoadAnimator(context, objectAnimation);

                var animator = (Animator) anim;

                animator.AnimationStart += (sender, args) =>
                {
                    if (AnimationStart != null)
                    {
                        AnimationStart();
                    }
                };
                animator.AnimationEnd += (sender, args) =>
                {
                    if (AnimatioEnd != null)
                    {
                        AnimatioEnd();
                    }
                };
            }
            else
            {
                anim = AnimationUtils.LoadAnimation(context, backupAnimation);
                var animation = (Animation) anim;
                animation.FillBefore = true;
                animation.FillAfter = true;

                animation.AnimationStart += (sender, args) =>
                {
                    if (AnimationStart != null)
                    {
                        AnimationStart();
                    }
                };

                animation.AnimationEnd += (sender, args) =>
                {
                    if (AnimatioEnd != null)
                    {
                        AnimatioEnd();
                    }
                };
            }
        }

        public int Duration { 
            set{ 
                if (anim is Animation)
                {
                    ((Animation) anim).Duration = value;
                }
                else
                {
                    if (anim is Animator)
                    {
                        ((Animator)anim).SetDuration(value);
                    }
                    else
                    {
                        Log.Error("CompatibilityAnimation", "Animation is neither of type Animator nor Animation");
                    }
                }
            }   
        }

        public int Delay
        {
            set
            {
                if (anim is Animation)
                {
                    ((Animation)anim).StartOffset = value;
                }
                else
                {
                    if (anim is Animator)
                    {
                        ((Animator)anim).StartDelay = value;
                    }
                    else
                    {
                        Log.Error("CompatibilityAnimation", "Animation is neither of type Animator nor Animation");
                    }
                }
            }
        }

        public void StartAnimation(View v)
        {
            if (anim is Animation)
            {
                v.StartAnimation((Animation)anim);
            }
            else
            {
                if (anim is Animator)
                {
                    var animator = (Animator)anim;
                    animator.SetTarget(v);
                    animator.Start();
                }
                else
                {
                    Log.Error("CompatibilityAnimation", "Animation is neither of type Animator nor Animation");
                }
            }
        }
        
    }
}