using AutoMapper;
using IEE.Infrastructure.DbContext;

using IEE.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEE.WebApp.Mappings
{
    public class DomainToViewModelMappingProfile 
    {
        private static readonly Lazy<DomainToViewModelMappingProfile> LazyInstance = new Lazy<DomainToViewModelMappingProfile>(() => new DomainToViewModelMappingProfile());

        public static DomainToViewModelMappingProfile Instance
        {
            get { return LazyInstance.Value; }
        }

        
        public DomainToViewModelMappingProfile()
        {
            //CreateMap<Banner, BannerViewModel>();
            //CreateMap<Category, CategoryViewModel>();
            //CreateMap<Post, PostViewModel>();
            //CreateMap<User, UserViewModel>();
            //CreateMap<Role, RoleViewModel>();
            //CreateMap<Setting, SettingViewModel>();
            //CreateMap<Testimonial, TestimonialViewModel>();
            //CreateMap<Council, CouncilViewModel>();
            //CreateMap<TeacherHighlight, TeacherHighlightViewModel>();
            //CreateMap<StudentHighlight, StudentHighlightViewModel>();
            //CreateMap<UniversityHighlight, UniversityHighlightViewModel>();
            //CreateMap<Calendar, CalendarViewModel>();
            //CreateMap<Consultant, ConsultantViewModel>();
        }

        public  void Configure()
        {
            Mapper.CreateMap<Banner, BannerViewModel>();
            Mapper.CreateMap<Category, CategoryViewModel>();
            Mapper.CreateMap<Post, PostViewModel>();
            Mapper.CreateMap<User, UserViewModel>();
            Mapper.CreateMap<Role, RoleViewModel>();
            Mapper.CreateMap<Setting, SettingViewModel>();
            Mapper.CreateMap<Testimonial, TestimonialViewModel>();
            Mapper.CreateMap<Council, CouncilViewModel>();
            Mapper.CreateMap<TeacherHighlight, TeacherHighlightViewModel>();
            Mapper.CreateMap<StudentHighlight, StudentHighlightViewModel>();
            Mapper.CreateMap<UniversityHighlight, UniversityHighlightViewModel>();
            Mapper.CreateMap<Calendar, CalendarViewModel>();
            Mapper.CreateMap<Consultant, ConsultantViewModel>();
        }

        //protected override void Configure()
        //{
        //    Mapper.CreateMap<Banner, BannerViewModel>();
        //    Mapper.CreateMap<Category, CategoryViewModel>();
        //    Mapper.CreateMap<Post, PostViewModel>();
        //    Mapper.CreateMap<User, UserViewModel>();
        //    Mapper.CreateMap<Role, RoleViewModel>();
        //    Mapper.CreateMap<Setting, SettingViewModel>();
        //    Mapper.CreateMap<Testimonial, TestimonialViewModel>();
        //    Mapper.CreateMap<Council, CouncilViewModel>();
        //    Mapper.CreateMap<TeacherHighlight, TeacherHighlightViewModel>();
        //    Mapper.CreateMap<StudentHighlight, StudentHighlightViewModel>();
        //    Mapper.CreateMap<UniversityHighlight, UniversityHighlightViewModel>();
        //    Mapper.CreateMap<Calendar, CalendarViewModel>();
        //    Mapper.CreateMap<Consultant, ConsultantViewModel>();
        //}
    }
}