using System;
using System.Collections.Generic;
using System.Linq;

namespace AntennaArray
{
    /** Antenna array design problem */
    public sealed class AntennaArray
    {
        /** Minimum spacing permitted between antennae. */
        public const double MinSpacing = 0.25;

        private readonly int _numberOfAntenna;
        private readonly double _steeringAngle;

        /**
         * Construct an antenna design problem.
         * @param n_ant Number of antennae in our array.
         * @param steering_ang Desired direction of the main beam in degrees.
         */
        public AntennaArray(int numberOfAntenna, double steeringAngle)
        {
            _numberOfAntenna = numberOfAntenna;
            _steeringAngle = steeringAngle;
        }
        
        /**
         * Rectangular bounds on the search space.
         * @return Vector b such that b[i][0] is the minimum permissible value of the
         * ith solution component and b[i][1] is the maximum.
         */
        public double[][] Bounds()
        {
            var bnds = new double[_numberOfAntenna][];
            double[] dimBnd = { 0.0, _numberOfAntenna / 2.0 };

            for (var i = 0; i < _numberOfAntenna; ++i)
            {
                bnds[i] = dimBnd;
            }

            return bnds;
        }
        
        /**
         * Check whether an antenna design lies within the problem's feasible
         * region.
         * A design is a vector of n_antennae anntena placements.
         * A placement is a distance from the left hand side of the antenna array.
         * A valid placement is one in which
         *   1) all antennae are separated by at least MIN_SPACING
         *   2) the aperture size (the maximum element of the array) is exactly
         *      n_antennae/2.
         */
        public bool IsValid(double[] design)
        {
            if (design.Length != _numberOfAntenna)
            {
                return false;
            }

            var des = design.OrderBy(v => v).ToArray();

            //Aperture size is exactly n_antennae/2
            if (Math.Abs(des[^1] - _numberOfAntenna / 2.0) > 1e-10)
            {
                return false;
            }

            //All antennae lie within the problem bounds
            for (var i = 0; i < des.Length - 1; ++i)
            {
                if (des[i] < Bounds()[i][0] || des[i] > Bounds()[i][1])
                {
                    return false;
                }
            }

            //All antennae are separated by at least MIN_SPACING
            for (var i = 0; i < des.Length - 1; ++i)
            {
                if (des[i + 1] - des[i] < MinSpacing)
                {
                    return false;
                }
            }

            return true;
        }

        /**
         * Evaluate an antenna design returning peak SSL.
         * Designs which violate problem constraints will be penalised with extremely
         * high costs.
         * @param design A valid antenna array design.
         */
        public double Evaluate(double[] design)
        {
            if (design.Length != _numberOfAntenna)
            {
                throw new Exception($"AntennaArray::evaluate called on design of the wrong size. Expected: {_numberOfAntenna}. Actual: {design.Length}");
            }

            if (!IsValid(design))
            {
                return double.MaxValue;
            }

            //Find all the peaks in power
            var peaks = new List<PowerPeak>();
            var prev = new PowerPeak(0.0, double.MaxValue);
            var current = new PowerPeak(0.0, ArrayFactor(design, 0.0));
            for (var elevation = 0.01; elevation <= 180.0; elevation += 0.01)
            {
                var next = new PowerPeak(elevation, ArrayFactor(design, elevation));
                if (current.Power >= prev.Power && current.Power >= next.Power)
                {
                    peaks.Add(current);
                }

                prev = current;
                current = next;
            }

            peaks.Add(new PowerPeak(180.0, ArrayFactor(design, 180.0)));

            peaks.Sort((l, r) => l.Power > r.Power ? -1 : 1);

            //No side-lobes case
            if (peaks.Count < 2)
            {
                return double.MinValue;
            }

            //Filter out main lobe and then return highest lobe level
            var distanceFromSteering = Math.Abs(peaks[0].Elevation - _steeringAngle);
            for (var i = 1; i < peaks.Count; ++i)
            {
                if (Math.Abs(peaks[i].Elevation - _steeringAngle) < distanceFromSteering)
                {
                    return peaks[0].Power;
                }
            }

            return peaks[1].Power;
        }

        private double ArrayFactor(double[] design, double elevation)
        {
            var steering = 2.0 * Math.PI * _steeringAngle / 360.0;
            elevation = 2.0 * Math.PI * elevation / 360.0;

            var sum = design.Sum(x => Math.Cos(2 * Math.PI * x * (Math.Cos(elevation) - Math.Cos(steering))));

            return 20.0 * Math.Log(Math.Abs(sum));
        }

        private class PowerPeak
        {
            public readonly double Elevation;
            public readonly double Power;

            public PowerPeak(double e, double p)
            {
                Elevation = e;
                Power = p;
            }
        }
    }
}
