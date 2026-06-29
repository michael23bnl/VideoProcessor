#FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["FileProcessingMicroservice.csproj", "./"]
#RUN dotnet restore "FileProcessingMicroservice.csproj"
#COPY . .
#RUN dotnet build "./FileProcessingMicroservice.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./FileProcessingMicroservice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "FileProcessingMicroservice.dll"]

#FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["VideoProcessingMicroservice.csproj", "./"]
#RUN dotnet restore "VideoProcessingMicroservice.csproj"
#COPY . .
#RUN dotnet build "./VideoProcessingMicroservice.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./VideoProcessingMicroservice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## Добавляем FFmpeg
## Добавляем FFmpeg
#FROM base AS ffmpeg
#ENV BIN=/usr/bin/
#ENV FFmpeg=/app/FFmpeg/
#ENV FFmpegBin=/app/FFmpeg/ffmpeg-master-latest-linux64-lgpl/bin
#ADD FFmpeg/ffmpeg-master-latest-linux64-lgpl.tar.xz $FFmpeg
#RUN rm $FFmpeg/ffmpeg-master-latest-linux64-lgpl.tar.xz \
#    && chmod +x $FFmpegBin/ffmpeg \
#    && chmod +x $FFmpegBin/ffprobe \
#    && ln -s $FFmpegBin/ffmpeg $BIN \
#    && ln -s $FFmpegBin/ffprobe $BIN
#
#FROM ffmpeg AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "VideoProcessingMicroservice.dll"]

# РАБОЧИЙ ВАРИАНТ

#FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["VideoProcessingMicroservice.csproj", "./"]
#RUN dotnet restore "VideoProcessingMicroservice.csproj"
#COPY . .
#RUN dotnet build "./VideoProcessingMicroservice.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./VideoProcessingMicroservice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## Добавляем FFmpeg
#FROM base AS ffmpeg
#USER root
#ENV BIN=/usr/bin/
#ENV FFmpeg=/app/FFmpeg/
#ENV FFmpegBin=/app/FFmpeg/ffmpeg-master-latest-linux64-lgpl/bin
#
## ADD автоматически распаковывает .tar.xz файл
#ADD FFmpeg/ffmpeg-master-latest-linux64-lgpl.tar.xz $FFmpeg
#
#RUN chmod +x $FFmpegBin/ffmpeg \
#    && chmod +x $FFmpegBin/ffprobe \
#    && ln -s $FFmpegBin/ffmpeg $BIN/ffmpeg \
#    && ln -s $FFmpegBin/ffprobe $BIN/ffprobe
#
#USER $APP_UID
#
#FROM ffmpeg AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "VideoProcessingMicroservice.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VideoProcessingMicroservice/VideoProcessingMicroservice.csproj", "VideoProcessingMicroservice/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "VideoProcessingMicroservice/VideoProcessingMicroservice.csproj"
COPY . .
WORKDIR "/src/VideoProcessingMicroservice"
RUN dotnet build "./VideoProcessingMicroservice.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VideoProcessingMicroservice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Добавляем FFmpeg
FROM base AS ffmpeg
USER root
ENV BIN=/usr/bin/
ENV FFmpeg=/app/FFmpeg/
ENV FFmpegBin=/app/FFmpeg/ffmpeg-master-latest-linux64-lgpl/bin

# ADD автоматически распаковывает .tar.xz файл
ADD VideoProcessingMicroservice/FFmpeg/ffmpeg-master-latest-linux64-lgpl.tar.xz $FFmpeg

RUN chmod +x $FFmpegBin/ffmpeg \
    && chmod +x $FFmpegBin/ffprobe \
    && ln -s $FFmpegBin/ffmpeg $BIN/ffmpeg \
    && ln -s $FFmpegBin/ffprobe $BIN/ffprobe

USER $APP_UID

FROM ffmpeg AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoProcessingMicroservice.dll"]