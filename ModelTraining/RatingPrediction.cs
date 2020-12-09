using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movie_recommendation.Entities
{
    public class RatingPrediction
    {
        [ColumnName("rating")]
        public float rating;

        [ColumnName("Score")]
        public float score;
    }
}
