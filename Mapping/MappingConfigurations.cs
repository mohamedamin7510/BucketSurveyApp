using BucketSurvey.Api.Contract.User;
using BucketSurvey.Api.Contract.Vote;

namespace BucketSurvey.Api.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {

        config.NewConfig<QuestionRequest, Question>()
            .Map(x => x.Answers, x => x.Answers.Select(y => new Answer { Content = y }));


        config.NewConfig<VoteAnswerRequest, VoteAnswers>()
            .Map(x => x.QuesitonId, x => x.QuestionId);


        config.NewConfig<(ApplicationUser user, IList<string> roles), UserResponse>()
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Roles, src => src.roles);


        config.NewConfig<UserRequest, ApplicationUser>()
            .Map(x => x.UserName, x => x.Email)
            .Map(x => x.EmailConfirmed, x => true);


        config.NewConfig<UpdateUserRequest, ApplicationUser>()
        .Map(x => x.UserName, x => x.Email);
                     


    }
}

