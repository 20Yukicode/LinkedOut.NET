﻿using LinkedOut.Common.Exception;
using LinkedOut.Common.Feign.User.Dto;
using LinkedOut.DB;
using LinkedOut.DB.Entity;
using LinkedOut.User.Domain.Enum;
using LinkedOut.User.Domain.Vo;
using LinkedOut.User.Helper;

namespace LinkedOut.User.Manager;

public class SubscribedManager
{
    private readonly LinkedOutContext _context;

    public SubscribedManager(LinkedOutContext context)
    {
        _context = context;
    }

    public (SubscribedState SubScribed, Subscribed? relation) GetRelation(int firstUserId, int secondUserId)
    {
        if (firstUserId == secondUserId)
        {
            return (SubscribedState.Same, null);
        }

        var relation = _context.Subscribeds.Select(o => o)
            .SingleOrDefault(o => o.FirstUserId == o.SecondUserId);
        return relation == null ? (SubscribedState.NoSubscribed, null) : (SubscribedState.SubScribed, relation);
    }

    //查找自己也就是unifiedId和别人的分值，看看要不要把别人推荐给你
    public double RecommendScore(int unifiedId, DB.Entity.User user, string userType)
    {
        var self = _context.Users.SingleOrDefault(o => o.UnifiedId == unifiedId);
        if (self == null)
        {
            throw new ApiException($"不存在id为{unifiedId}的用户");
        }

        var selfInfo = _context.UserInfos.Single(o => o.UnifiedId == unifiedId);

        var score = 0.0;
        //别人的基本信息
        var userId = user.UnifiedId;

        //对于不同角色，推荐的角度不同
        switch (userType)
        {
            case "user":
                var userInfo = _context.UserInfos.Single(o => o.UnifiedId == userId);
                score += SimilarityHelper.SimilarScoreCos(user.BriefInfo, self.BriefInfo);
                score += SimilarityHelper.SimilarScoreCos(userInfo.PrePosition, selfInfo.PrePosition);
                score /= 2;
                break;
            case "company":
                score += SimilarityHelper.SimilarScoreCos(user.BriefInfo, self.BriefInfo);
                break;
            case "school":
                break;
        }

        return score;
    }

    public List<UserDto> GetSubscribeUserIds(int unifiedId)
    {
        return _context.Subscribeds
            .Where(o => o.FirstUserId == unifiedId)
            .Join(_context.Users,
                s => s.SecondUserId,
                u => u.UnifiedId,
                (s, u) => new UserDto
                {
                    BriefInfo = u.BriefInfo,
                    PictureUrl = u.Avatar,
                    TrueName = u.TrueName,
                    UnifiedId = u.UnifiedId,
                    UserType = u.UserType
                }).ToList();
    }

}