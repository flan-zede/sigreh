using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace sigreh.Dtos
{
    public class NotificationResponse
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationResponse>();
        }
    }
}
