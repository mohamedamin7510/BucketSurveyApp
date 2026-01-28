
using BucketSurvey.Api.Contract.Answer;
using BucketSurvey.Api.Contract.Question;
using BucketSurvey.Api.Contract.Vote;
using System.Data;

namespace BucketSurvey.Api.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionRequest, Question>()
            .Map(x=>x.Answers , x=> x.Answers.Select(y =>new Answer { Content = y}));

        config.NewConfig<VoteAnswerRequest, VoteAnswers>()
            .Map(x=>x.QuesitonId  , x=>x.QuestionId);

    }
}

