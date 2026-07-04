using AutoMapper;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Mapper;

/// <summary>
/// Perfil de mapeamento do AutoMapper para conversão entre entidades e DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserProfileDto>();
        CreateMap<TaskItem, TaskDto>();
        CreateMap<Note, NoteDto>();
        CreateMap<PendingLog, PendingLogDto>();
        CreateMap<Attachment, AttachmentDto>();
        CreateMap<VisualPdfRecord, VisualPdfRecordDto>();
    }
}
