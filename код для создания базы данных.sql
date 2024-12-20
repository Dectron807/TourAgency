USE [master]
GO
/****** Object:  Database [db_1]    Script Date: 18.09.2024 20:34:12 ******/
CREATE DATABASE [db_1]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'db_1', FILENAME = N'C:\123\db_1.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'db_1_log', FILENAME = N'C:\123\db_1_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [db_1] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [db_1].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [db_1] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [db_1] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [db_1] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [db_1] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [db_1] SET ARITHABORT OFF 
GO
ALTER DATABASE [db_1] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [db_1] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [db_1] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [db_1] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [db_1] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [db_1] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [db_1] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [db_1] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [db_1] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [db_1] SET  DISABLE_BROKER 
GO
ALTER DATABASE [db_1] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [db_1] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [db_1] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [db_1] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [db_1] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [db_1] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [db_1] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [db_1] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [db_1] SET  MULTI_USER 
GO
ALTER DATABASE [db_1] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [db_1] SET DB_CHAINING OFF 
GO
ALTER DATABASE [db_1] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [db_1] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [db_1] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [db_1] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [db_1] SET QUERY_STORE = OFF
GO
USE [db_1]
GO
/****** Object:  Table [dbo].[Авиакомпании]    Script Date: 18.09.2024 20:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Авиакомпании](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Название] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Договор]    Script Date: 18.09.2024 20:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Договор](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Тур_id] [int] NOT NULL,
	[Дата_Заключения] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[История]    Script Date: 18.09.2024 20:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[История](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Клиент_id] [int] NULL,
	[Тур_id] [int] NULL,
	[Дата_Операции] [datetime] NULL,
	[Операция] [nvarchar](50) NULL,
	[Количество_Билетов] [int] NULL,
	[Логин] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Клиент]    Script Date: 18.09.2024 20:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Клиент](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Фамилия] [varchar](255) NOT NULL,
	[Имя] [varchar](255) NOT NULL,
	[Отчество] [varchar](255) NULL,
	[Пол] [varchar](35) NOT NULL,
	[Номер_Паспорта] [varchar](35) NOT NULL,
	[Дата_Рождения] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[МоиТуры]    Script Date: 18.09.2024 20:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[МоиТуры](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Клиент_id] [int] NOT NULL,
	[Тур_id] [int] NOT NULL,
	[Дата_Заключения] [datetime] NOT NULL,
	[Количество_Билетов] [int] NULL,
	[Логин] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Отель]    Script Date: 18.09.2024 20:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Отель](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Название] [varchar](255) NOT NULL,
	[Цена_Компании] [int] NOT NULL,
	[Цена_Клиента] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Перелет]    Script Date: 18.09.2024 20:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Перелет](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Авиакомпании_id] [int] NOT NULL,
	[Цена_Компании] [int] NOT NULL,
	[Цена_Клиента] [int] NOT NULL,
	[Страна_Отправления_id] [int] NOT NULL,
	[Пункт_Назначения_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Пользователи]    Script Date: 18.09.2024 20:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Пользователи](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Логин] [varchar](255) NOT NULL,
	[Пароль] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Логин] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Страна]    Script Date: 18.09.2024 20:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Страна](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Название] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Тур]    Script Date: 18.09.2024 20:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Тур](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Отель_id] [int] NOT NULL,
	[Страна_id] [int] NOT NULL,
	[Авиарейса_Вылета_id] [int] NOT NULL,
	[Авиарейса_Прилёта_id] [int] NOT NULL,
	[Дата_Начала] [date] NOT NULL,
	[Дата_Окончания] [date] NOT NULL,
	[Стоимость_Услуг_Компании] [int] NOT NULL,
	[Количество_Свободных_Мест] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Турист]    Script Date: 18.09.2024 20:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Турист](
	[Клиент_id] [int] NOT NULL,
	[Договор_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Клиент_id] ASC,
	[Договор_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Договор]  WITH CHECK ADD FOREIGN KEY([Тур_id])
REFERENCES [dbo].[Тур] ([id])
GO
ALTER TABLE [dbo].[История]  WITH CHECK ADD FOREIGN KEY([Клиент_id])
REFERENCES [dbo].[Клиент] ([id])
GO
ALTER TABLE [dbo].[История]  WITH CHECK ADD FOREIGN KEY([Тур_id])
REFERENCES [dbo].[Тур] ([id])
GO
ALTER TABLE [dbo].[МоиТуры]  WITH CHECK ADD FOREIGN KEY([Клиент_id])
REFERENCES [dbo].[Клиент] ([id])
GO
ALTER TABLE [dbo].[МоиТуры]  WITH CHECK ADD FOREIGN KEY([Логин])
REFERENCES [dbo].[Пользователи] ([Логин])
GO
ALTER TABLE [dbo].[МоиТуры]  WITH CHECK ADD FOREIGN KEY([Тур_id])
REFERENCES [dbo].[Тур] ([id])
GO
ALTER TABLE [dbo].[Перелет]  WITH CHECK ADD FOREIGN KEY([Авиакомпании_id])
REFERENCES [dbo].[Авиакомпании] ([id])
GO
ALTER TABLE [dbo].[Перелет]  WITH CHECK ADD FOREIGN KEY([Пункт_Назначения_id])
REFERENCES [dbo].[Страна] ([id])
GO
ALTER TABLE [dbo].[Перелет]  WITH CHECK ADD FOREIGN KEY([Страна_Отправления_id])
REFERENCES [dbo].[Страна] ([id])
GO
ALTER TABLE [dbo].[Тур]  WITH CHECK ADD FOREIGN KEY([Авиарейса_Вылета_id])
REFERENCES [dbo].[Перелет] ([id])
GO
ALTER TABLE [dbo].[Тур]  WITH CHECK ADD FOREIGN KEY([Авиарейса_Прилёта_id])
REFERENCES [dbo].[Перелет] ([id])
GO
ALTER TABLE [dbo].[Тур]  WITH CHECK ADD FOREIGN KEY([Отель_id])
REFERENCES [dbo].[Отель] ([id])
GO
ALTER TABLE [dbo].[Тур]  WITH CHECK ADD FOREIGN KEY([Страна_id])
REFERENCES [dbo].[Страна] ([id])
GO
ALTER TABLE [dbo].[Турист]  WITH CHECK ADD FOREIGN KEY([Договор_id])
REFERENCES [dbo].[Договор] ([id])
GO
ALTER TABLE [dbo].[Турист]  WITH CHECK ADD FOREIGN KEY([Клиент_id])
REFERENCES [dbo].[Клиент] ([id])
GO
ALTER TABLE [dbo].[Отель]  WITH CHECK ADD CHECK  (([Цена_Клиента]>(0)))
GO
ALTER TABLE [dbo].[Отель]  WITH CHECK ADD CHECK  (([Цена_Компании]<=(0)))
GO
ALTER TABLE [dbo].[Тур]  WITH CHECK ADD CHECK  (([Стоимость_Услуг_Компании]>(0)))
GO
ALTER TABLE [dbo].[Тур]  WITH CHECK ADD  CONSTRAINT [CK_Tur_Количество_Свободных_Мест] CHECK  (([Количество_Свободных_Мест]>=(0)))
GO
ALTER TABLE [dbo].[Тур] CHECK CONSTRAINT [CK_Tur_Количество_Свободных_Мест]
GO
USE [master]
GO
ALTER DATABASE [db_1] SET  READ_WRITE 
GO
