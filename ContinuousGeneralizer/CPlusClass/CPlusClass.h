// CPlusClass.h

#pragma once


using namespace System;

#include <vector>
using namespace std;

//#include <Eigen/Sparse>
#include <C:\MyWork\DailyWork\ContinuousGeneralisation\ContinuousGeneralizer\Citations\eigen\Eigen/Sparse>
using namespace Eigen;

int xid( int id ) { return 2*id; }
int yid( int id ) { return 2*id + 1; }







double CalAxisAngle(double dblX, double dblY) {
	double dblAbsX = Math::Abs(dblX);
	double dblAbsY = Math::Abs(dblY);
	double dblAngle = Math::Atan(dblAbsY / dblAbsX);

    double dblAxisAngle = 0;
    if (dblX >= 0 && dblY >= 0)  //第一象限
    {
        dblAxisAngle = dblAngle;
    }
    else if (dblX < 0 && dblY >= 0)  //第二象限
    {
        dblAxisAngle = Math::PI - dblAngle;
    }
    else if (dblX < 0 && dblY < 0)  //第三象限
    {
        dblAxisAngle = Math::PI + dblAngle;
    }
    else if (dblX >= 0 && dblY < 0)  //第四象限
    {
        dblAxisAngle = 2 * Math::PI - dblAngle;
    }
    return dblAxisAngle;
}
/// <summary>计算夹角(逆时针)</summary>
/// <returns>夹角弧度值</returns>
/// <remarks>用求差的方法求夹角，可区分夹角的方向</remarks>
double CalAngle_Counterclockwise(double dblX1, double dblY1, double dblX2, double dblY2, double dblX3, double dblY3) {
    //计算始向量与坐标横轴的夹角
    double dblpreDiffX = dblX1 - dblX2;
    double dblpreDiffY = dblY1 - dblY2;
    double dblStartAngle = CalAxisAngle(dblpreDiffX, dblpreDiffY);

    //计算末向量与坐标横轴的夹角
    double dblfolDiffX = dblX3 - dblX2;
    double dblfolDiffY = dblY3 - dblY2;
    double dblEndAngle = CalAxisAngle(dblfolDiffX, dblfolDiffY);

    //夹角差
    double dblAngleDiff = dblEndAngle - dblStartAngle;
    if (dblAngleDiff<0) {
        dblAngleDiff = dblAngleDiff + 2 * Math::PI;
    }

    return dblAngleDiff;
}














namespace CPlusClass {

	public ref class Matrix
	{
	public:

		int add(int a, int b)
		{
			int c = a + b;
			return c;
		}

		static int leastSquaresAdjust(int numPoints,
			cli::array<double> ^x,
			cli::array<double> ^y,
			cli::array<bool> ^isFixed,
			cli::array<double> ^length,
			cli::array<double> ^angle,
			cli::array<double> ^weight,
			double threshold,
			int nummaxiterative,
			cli::array<double> ^xout,
			cli::array<double> ^yout
		) {
			typedef Triplet<double> Trip;
			//array<double> st;

			int *variableIndex = new int[numPoints];
			vector<int> lengthConstraints;
			vector<int> angleConstraints;
			int numUnknownPoints = 0;
			int currentIndex = 0;
			for (int i = 0; i < numPoints; ++i) {
				xout[i] = x[i];
				yout[i] = y[i];
				if (isFixed[i]) {
					variableIndex[i] = -1; // this variable doesn't have an index in the matrix
				}
				else {
					numUnknownPoints++;
					variableIndex[i] = currentIndex;
					currentIndex += 1;
				}
			}
			for (int i = 0; i < numPoints - 1; ++i) {
				if (!isFixed[i] || !isFixed[i + 1]) lengthConstraints.push_back(i);
			}
			for (int i = 0; i < numPoints - 2; ++i) {
				if (!isFixed[i] || !isFixed[i + 1] || !isFixed[i + 2]) angleConstraints.push_back(i);
			}

			int numUnknownLengths = lengthConstraints.size();
			int numUnknownAngles = angleConstraints.size();
			int numConstraints = numUnknownLengths + numUnknownAngles;

			vector<Trip> tripsweight;
			for (int i = 0; i < numConstraints; ++i) {
				double value = weight[i];
				tripsweight.push_back(Trip(i, i, value));
			}

			SparseMatrix<double> P(numConstraints, numConstraints); // construct matrix of correct size
			P.setFromTriplets(tripsweight.begin(), tripsweight.end()); // put the correct values in the matrix

			bool done = false;
			int intInterativeNum = 0;
			do {
				vector<Trip> trips;

				int numCols = 2 * numUnknownPoints;
				int numRows = numUnknownAngles + numUnknownLengths;

				VectorXd rhs(numRows); // right-hand-side vector

				int currentRow = 0;
				// Length constraints
				for (vector<int>::iterator index = lengthConstraints.begin(); index != lengthConstraints.end(); ++index) {
					int i = *index;

					double xdiff = xout[i + 1] - xout[i];
					double ydiff = yout[i + 1] - yout[i];
					double s = sqrt(xdiff*xdiff + ydiff*ydiff);

					if (!isFixed[i]) {
						double value = -(xout[i + 1] - xout[i]) / s;
						trips.push_back(Trip(currentRow, xid(variableIndex[i]), value));
						value = -(yout[i + 1] - yout[i]) / s;
						trips.push_back(Trip(currentRow, yid(variableIndex[i]), value));
					}
					if (!isFixed[i + 1]) {
						double value = (xout[i + 1] - xout[i]) / s;
						trips.push_back(Trip(currentRow, xid(variableIndex[i + 1]), value));
						value = (yout[i + 1] - yout[i]) / s;
						trips.push_back(Trip(currentRow, yid(variableIndex[i + 1]), value));
					}
					rhs[currentRow] = length[i] - s;
					currentRow++;
				}




				// Angle constraint
				for (vector<int>::iterator index = angleConstraints.begin(); index != angleConstraints.end(); ++index) {
					int i = *index;

					double xdiff1 = xout[i + 1] - xout[i];
					double ydiff1 = yout[i + 1] - yout[i];
					double s1Squared = xdiff1*xdiff1 + ydiff1*ydiff1;
					double xdiff2 = xout[i + 2] - xout[i + 1];
					double ydiff2 = yout[i + 2] - yout[i + 1];
					double s2Squared = xdiff2*xdiff2 + ydiff2*ydiff2;

					if (!isFixed[i]) {
						double value = -(yout[i + 1] - yout[i]) / s1Squared;
						trips.push_back(Trip(currentRow, xid(variableIndex[i]), value));
						value = (xout[i + 1] - xout[i]) / s1Squared;
						trips.push_back(Trip(currentRow, yid(variableIndex[i]), value));
					}
					if (!isFixed[i + 1]) {
						double value = (yout[i + 2] - yout[i + 1]) / s2Squared + (yout[i + 1] - yout[i]) / s1Squared;
						trips.push_back(Trip(currentRow, xid(variableIndex[i + 1]), value));
						value = -(xout[i + 2] - xout[i + 1]) / s2Squared - (xout[i + 1] - xout[i]) / s1Squared;
						trips.push_back(Trip(currentRow, yid(variableIndex[i + 1]), value));
					}
					if (!isFixed[i + 2]) {
						double value = -(yout[i + 2] - yout[i + 1]) / s2Squared;
						trips.push_back(Trip(currentRow, xid(variableIndex[i + 2]), value));
						value = (xout[i + 2] - xout[i + 1]) / s2Squared;
						trips.push_back(Trip(currentRow, yid(variableIndex[i + 2]), value));
					}
					double newAngle = CalAngle_Counterclockwise(xout[i], yout[i], xout[i + 1], yout[i + 1], xout[i + 2], yout[i + 2]);
					rhs[currentRow] = angle[i] - newAngle;
					currentRow++;

				}

				SparseMatrix<double> A(numRows, numCols); // construct matrix of correct size
				A.setFromTriplets(trips.begin(), trips.end()); // put the correct values in the matrix

				SparseMatrix<double> AtPA = A.transpose() *P* A; // compute A^T A

				VectorXd AtPl = A.transpose() *P* rhs; // compute A^T l

				// solve for x
				SimplicialLDLT<SparseMatrix<double> > solver(AtPA);
				ComputationInfo info = solver.info();
				VectorXd solution = solver.solve(AtPl);

				double ax[1000];
				int intXCount = 0;

				// update xout and yout
				for (int i = 0; i < numPoints; ++i) {
					if (!isFixed[i]) {
						double xChange = solution[xid(variableIndex[i])];
						xout[i] += xChange;
						double yChange = solution[yid(variableIndex[i])];
						yout[i] += yChange;
					}
				}

				// done if change is small
				double dblnorm = solution.norm();
				done = solution.norm() <= threshold;
				//done=true;
				if (done == true)
				{
					int kk = 5;
				}
				intInterativeNum += 1;
				if (intInterativeNum >= nummaxiterative)
				{
					break;
				}
			} while (!done);

			delete[] variableIndex;
			return 42;




		}





		// TODO: 在此处添加此类的方法。
	};
}
