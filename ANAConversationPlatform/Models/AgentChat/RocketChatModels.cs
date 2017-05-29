using Newtonsoft.Json;
using System.Collections.Generic;

namespace ANAConversationPlatform.Models.AgentChat
{
    public class ResponseBase
    {
        [JsonProperty("status")]
        public object Status { get; set; }
    }
    public class LoginData
    {

        [JsonProperty("authToken")]
        public string AuthToken { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
    public class LoginResponse : ResponseBase
    {
        [JsonProperty("data")]
        public LoginData Data { get; set; }
    }
    public class CreateUserRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("customFields")]
        public Dictionary<string, object> CustomFields { get; set; }

        [JsonProperty("joinDefaultChannels")]
        public bool JoinDefaultChannels { get; set; }

        [JsonProperty("requirePasswordChange")]
        public bool RequirePasswordChange { get; set; }

        [JsonProperty("sendWelcomeEmail")]
        public bool SendWelcomeEmail { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("roles")]
        public string[] Roles { get; set; }
    }
    public class Password
    {
        [JsonProperty("bcrypt")]
        public string Bcrypt { get; set; }
    }
   
    public class Email
    {

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("verificationTokens")]
        public VerificationToken[] VerificationTokens { get; set; }
    }
    public class User : UserBase
    {
        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("services")]
        public Services Services { get; set; }

        [JsonProperty("emails")]
        public Email[] Emails { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("roles")]
        public string[] Roles { get; set; }

        [JsonProperty("_updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("customFields")]
        public Dictionary<string, object> CustomFields { get; set; }
    }
    public class UserResponse : ResponseBase
    {
        public User User { get; set; }
    }
  
    public class LoginToken
    {

        [JsonProperty("hashedToken")]
        public string HashedToken { get; set; }

        [JsonProperty("when")]
        public string When { get; set; }
    }

    public class Resume
    {

        [JsonProperty("loginTokens")]
        public LoginToken[] LoginTokens { get; set; }
    }

    public class VerificationToken
    {

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("when")]
        public string When { get; set; }
    }

    public class Services
    {

        [JsonProperty("password")]
        public Password Password { get; set; }

        [JsonProperty("resume")]
        public Resume Resume { get; set; }

        [JsonProperty("email")]
        public Email Email { get; set; }
    }

    public class Profile
    {

        [JsonProperty("guest")]
        public bool Guest { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }

    public class VisitorEmail
    {

        [JsonProperty("address")]
        public string Address { get; set; }
    }

    public class Preferences
    {

        [JsonProperty("newRoomNotification")]
        public string NewRoomNotification { get; set; }

        [JsonProperty("newMessageNotification")]
        public string NewMessageNotification { get; set; }

        [JsonProperty("useEmojis")]
        public bool UseEmojis { get; set; }

        [JsonProperty("convertAsciiEmoji")]
        public bool ConvertAsciiEmoji { get; set; }

        [JsonProperty("saveMobileBandwidth")]
        public bool SaveMobileBandwidth { get; set; }

        [JsonProperty("collapseMediaByDefault")]
        public bool CollapseMediaByDefault { get; set; }

        [JsonProperty("unreadRoomsMode")]
        public bool UnreadRoomsMode { get; set; }

        [JsonProperty("autoImageLoad")]
        public bool AutoImageLoad { get; set; }

        [JsonProperty("emailNotificationMode")]
        public string EmailNotificationMode { get; set; }

        [JsonProperty("unreadAlert")]
        public bool UnreadAlert { get; set; }

        [JsonProperty("desktopNotificationDuration")]
        public int DesktopNotificationDuration { get; set; }

        [JsonProperty("viewMode")]
        public int ViewMode { get; set; }

        [JsonProperty("hideUsernames")]
        public bool HideUsernames { get; set; }

        [JsonProperty("hideRoles")]
        public bool HideRoles { get; set; }

        [JsonProperty("hideAvatars")]
        public bool HideAvatars { get; set; }

        [JsonProperty("hideFlexTab")]
        public bool HideFlexTab { get; set; }

        [JsonProperty("highlights")]
        public object[] Highlights { get; set; }

        [JsonProperty("sendOnEnter")]
        public string SendOnEnter { get; set; }
    }

    public class Settings
    {

        [JsonProperty("preferences")]
        public Preferences Preferences { get; set; }
    }

    public class UserDetail : User
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastLogin")]
        public string LastLogin { get; set; }

        [JsonProperty("statusConnection")]
        public string StatusConnection { get; set; }

        [JsonProperty("utcOffset")]
        public double? UtcOffset { get; set; }

        [JsonProperty("statusDefault")]
        public string StatusDefault { get; set; }

        [JsonProperty("department")]
        public object Department { get; set; }

        [JsonProperty("joinDefaultChannels")]
        public bool? JoinDefaultChannels { get; set; }

        [JsonProperty("userAgent")]
        public string UserAgent { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("profile")]
        public Profile Profile { get; set; }

        [JsonProperty("visitorEmails")]
        public VisitorEmail[] VisitorEmails { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("settings")]
        public Settings Settings { get; set; }

        [JsonProperty("operator")]
        public bool? Operator { get; set; }

        [JsonProperty("statusLivechat")]
        public string StatusLivechat { get; set; }

        [JsonProperty("livechatCount")]
        public int? LivechatCount { get; set; }

        [JsonProperty("avatarOrigin")]
        public string AvatarOrigin { get; set; }
    }

    public class ListUserResponse
    {

        [JsonProperty("users")]
        public UserDetail[] Users { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class UserBase
    {

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
    public class Message
    {

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("parseUrls")]
        public bool ParseUrls { get; set; }

        [JsonProperty("groupable")]
        public bool Groupable { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("u")]
        public UserBase User { get; set; }

        [JsonProperty("rid")]
        public string Rid { get; set; }

        [JsonProperty("_updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }
    }
    public class PostMessageResponse : ResponseBase
    {
        [JsonProperty("ts")]
        public long Ts { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }
    }

    public class Im
    {

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("t")]
        public string T { get; set; }

        [JsonProperty("usernames")]
        public string[] Usernames { get; set; }

        [JsonProperty("msgs")]
        public int Msgs { get; set; }

        [JsonProperty("u")]
        public UserBase U { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("ro")]
        public bool Ro { get; set; }

        [JsonProperty("sysMes")]
        public bool SysMes { get; set; }

        [JsonProperty("_updatedAt")]
        public string UpdatedAt { get; set; }
    }

    public class IMListResponse : ResponseBase
    {

        [JsonProperty("ims")]
        public Im[] Ims { get; set; }
    }

    public class MessageField
    {

        [JsonProperty("short")]
        public bool Short { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Attachment
    {

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("thumb_url")]
        public string ThumbUrl { get; set; }

        [JsonProperty("message_link")]
        public string MessageLink { get; set; }

        [JsonProperty("collapsed")]
        public bool Collapsed { get; set; }

        [JsonProperty("author_name")]
        public string AuthorName { get; set; }

        [JsonProperty("author_link")]
        public string AuthorLink { get; set; }

        [JsonProperty("author_icon")]
        public string AuthorIcon { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_link")]
        public string TitleLink { get; set; }

        [JsonProperty("title_link_download")]
        public string TitleLinkDownload { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("audio_url")]
        public string AudioUrl { get; set; }

        [JsonProperty("video_url")]
        public string VideoUrl { get; set; }

        [JsonProperty("fields")]
        public MessageField[] Fields { get; set; }
    }

    public class PostMessageRequest
    {

        [JsonProperty("roomId")]
        public string RoomId { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("emoji")]
        public string Emoji { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("attachments")]
        public Attachment[] Attachments { get; set; }
    }
    public class RocketChatCallback
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("bot")]
        public bool Bot { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("channel_name")]
        public object ChannelName { get; set; }

        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
