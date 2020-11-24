﻿using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.DataAccess.Entities
{
	public class PostComment : BasicCommentData
	{
		[Key]
		public Guid Id { get; set; }
	}
}
