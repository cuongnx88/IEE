using AutoMapper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IEE.ViewModel;
using IEE.Infrastructure.DbContext;

namespace IEE.Web.Mappings
{
    public class ViewModelToDomainMappingProfile 
    {
        private static readonly Lazy<ViewModelToDomainMappingProfile> LazyInstance = new Lazy<ViewModelToDomainMappingProfile>(() => new ViewModelToDomainMappingProfile());

        public static ViewModelToDomainMappingProfile Instance
        {
            get { return LazyInstance.Value; }
        }

        public ViewModelToDomainMappingProfile()
        {
            //CreateMap<BannerViewModel, Banner>();
            //CreateMap<CategoryViewModel, Category>();
            //CreateMap<PostViewModel, Post>();
            //CreateMap<UserViewModel, User>();
            //CreateMap<RoleViewModel, Role>();
            //CreateMap<TestimonialViewModel, Testimonial>();
            //CreateMap<SettingViewModel, Setting>();
            //CreateMap<ConsultantViewModel, Consultant>();
            //CreateMap<TestimonialViewModel, Testimonial>();
            //CreateMap<CouncilViewModel, Council>();
            //CreateMap<TeacherHighlightViewModel, TeacherHighlight>();
            //CreateMap<StudentHighlightViewModel, StudentHighlight>();
            //CreateMap<UniversityHighlightViewModel, UniversityHighlight>();
            //CreateMap<CalendarViewModel, Calendar>();
        }
        public void Configure()
        {
            Mapper.CreateMap<BannerViewModel, Banner>();
            Mapper.CreateMap<CategoryViewModel, Category>();
            Mapper.CreateMap<PostViewModel, Post>();
            Mapper.CreateMap<UserViewModel, User>();
            Mapper.CreateMap<RoleViewModel, Role>();
            Mapper.CreateMap<TestimonialViewModel, Testimonial>();
            Mapper.CreateMap<SettingViewModel, Setting>();
            Mapper.CreateMap<ConsultantViewModel, Consultant>();
            Mapper.CreateMap<TestimonialViewModel, Testimonial>();
            Mapper.CreateMap<CouncilViewModel, Council>();
            Mapper.CreateMap<TeacherHighlightViewModel, TeacherHighlight>();
            Mapper.CreateMap<StudentHighlightViewModel, StudentHighlight>();
            Mapper.CreateMap<UniversityHighlightViewModel, UniversityHighlight>();
            Mapper.CreateMap<CalendarViewModel, Calendar>();
            Mapper.CreateMap<SATAccountRegisterModel, User>();
        }
    }
}