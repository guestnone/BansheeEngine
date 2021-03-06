﻿//********************************** Banshee Engine (www.banshee3d.com) **************************************************//
//**************** Copyright (c) 2016 Marko Pintera (marko.pintera@gmail.com). All rights reserved. **********************//
using bs;

namespace bs.Editor
{
    /** @addtogroup Inspectors
     *  @{
     */

    /// <summary>
    /// Renders an inspector for the <see cref="SphericalJoint"/> component.
    /// </summary>
    [CustomInspector(typeof(SphericalJoint))]
    internal class SphericalJointInspector : JointInspector
    {
        private GUIToggleField enableLimitField = new GUIToggleField(new LocEdString("Enable limit"));
        private LimitConeRangeGUI limitGUI;

        private GUILayoutX limitLayout;

        /// <inheritdoc/>
        protected internal override void Initialize()
        {
            SphericalJoint joint = InspectedObject as SphericalJoint;

            if (joint != null)
                BuildGUI(joint);
        }

        /// <inheritdoc/>
        protected internal override InspectableState Refresh()
        {
            SphericalJoint joint = InspectedObject as SphericalJoint;
            if (joint == null)
                return InspectableState.NotModified;

            Refresh(joint);

            InspectableState oldState = modifyState;
            if (modifyState.HasFlag(InspectableState.Modified))
                modifyState = InspectableState.NotModified;

            return oldState;
        }

        /// <summary>
        /// Creates GUI elements for fields specific to the spherical joint.
        /// </summary>
        protected void BuildGUI(SphericalJoint joint)
        {
            enableLimitField.OnChanged += x =>
            {
                joint.SetFlag(SphericalJointFlag.Limit, x);
                MarkAsModified();
                ConfirmModify();

                ToggleLimitFields(x);
            };

            Layout.AddElement(enableLimitField);
            limitLayout = Layout.AddLayoutX();
            {
                limitLayout.AddSpace(10);

                GUILayoutY limitContentsLayout = limitLayout.AddLayoutY();
                limitGUI = new LimitConeRangeGUI(joint.Limit, limitContentsLayout, Persistent);
                limitGUI.OnChanged += (x, y) =>
                {
                    joint.Limit = x;
                    joint.Limit.SetBase(y);

                    MarkAsModified();
                };
                limitGUI.OnConfirmed += ConfirmModify;
            }

            ToggleLimitFields(joint.HasFlag(SphericalJointFlag.Limit));

            base.BuildGUI(joint, true);
        }

        /// <summary>
        /// Updates all GUI elements from current values in the joint.
        /// </summary>
        /// <param name="joint">Joint to update the GUI from.</param>
        protected void Refresh(SphericalJoint joint)
        {
            bool enableLimit = joint.HasFlag(SphericalJointFlag.Limit);
            if (enableLimitField.Value != enableLimit)
            {
                enableLimitField.Value = enableLimit;
                ToggleLimitFields(enableLimit);
            }

            limitGUI.Limit = joint.Limit;

            base.Refresh(joint);
        }

        /// <summary>
        /// Hides or shows limit property GUI elements.
        /// </summary>
        /// <param name="enable">True to show, false to hide.</param>
        private void ToggleLimitFields(bool enable)
        {
            limitLayout.Active = enable;
        }
    }

    /** @} */
}