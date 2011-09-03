﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teudu.InfoDisplay
{
    public struct Arm
    {
        private const double SCALE_FACTOR = 0.1;
        public double HandX, HandY, HandZ;
        public double ElbowX, ElbowY, ElbowZ;
        public double ShoulderX, ShoulderY, ShoulderZ;

        public bool ArmAlmostStraight
        {
            get { return ((MaxArmSpan - CurrentArmSpan) <= 20); } //TODO: Adjust alpha
        }

        public double MaxArmSpan
        {
            get 
            {
                double foreArmLength = Math.Sqrt(
                    Math.Pow(HandX - ElbowX, 2) + 
                    Math.Pow(HandY - ElbowY, 2) + 
                    Math.Pow(HandZ - ElbowZ, 2)
                    );

                double backArmLength = Math.Sqrt(
                    Math.Pow(ShoulderX - ElbowX, 2) +
                    Math.Pow(ShoulderY - ElbowY, 2) +
                    Math.Pow(ShoulderZ - ElbowZ, 2)
                    );

                return foreArmLength + backArmLength;
            }
        }

        public double HandOffsetX
        {
            get { return HandX * SCALE_FACTOR; }
        }

        public double HandOffsetY
        {
            get { return HandY * SCALE_FACTOR; }
        }

        public double CurrentArmSpan
        {
            get
            {
                return Math.Sqrt(
                    Math.Pow(HandX - ShoulderX, 2) +
                    Math.Pow(HandY - ShoulderY, 2) +
                    Math.Pow(HandZ - ShoulderZ, 2)
                    );
            }
        }

    }

    public struct Torso
    {
        double torsoY;
        public double Y
        {
            get { return this.torsoY; }
            set { this.torsoY = value; }
        }
    }
}
