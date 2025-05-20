using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;

namespace Growell_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        [HttpPost("chat")]
        public IActionResult Chat([FromBody] UserMessage userMessage)
        {
            if (userMessage == null || string.IsNullOrWhiteSpace(userMessage.Message))
            {
                return BadRequest(new { reply = "يرجى إدخال رسالة صحيحة." });
            }

            var botReply = GetBotResponse(userMessage.Message);

            return Ok(new { reply = botReply });
        }

        private string GetBotResponse(string message)
        {
            // تحويل الرسالة إلى حروف صغيرة موحدة لدعم حالة الأحرف المختلفة
            var normalizedMessage = message.ToLowerInvariant();

            // تحسين البحث العربي باستخدام أقرب معنى للكلمات
            if (ContainsWord(normalizedMessage, new[] { "hello", "hi" }))
                return "Hello, How Can I Help you!";

            if (ContainsWord(normalizedMessage, new[] { "test" }))
                return "You Can Start by Answering Questions...";

            if (ContainsWord(normalizedMessage, new[] { "thanks", "thank you" }))
                return "You're Welcome! If you need any Other help Let, me Know";

            if (ContainsWord(normalizedMessage, new[] { "مرحبا", "اهلا" }))
                return "مرحبًا، كيف يمكنني مساعدتك؟";

            if (ContainsWord(normalizedMessage, new[] { "اختبار" }))
                return "يمكنك البدء بالإجابة على الأسئلة...";

            if (ContainsWord(normalizedMessage, new[] { "شكرا", "شكرًا" }))
                return "عفوًا! إذا كنت بحاجة إلى أي مساعدة أخرى، أخبرني.";

            if (ContainsWord(normalizedMessage, new[] { "أريد بدء الاختبار" }))
                return "يرجى الإجابة على الأسئلة التالية لتقييم حالتك.";

            if (ContainsWord(normalizedMessage, new[] { "كم عمرك" }))
                return "يرجى إدخال عمرك بالأرقام.";

            if (ContainsWord(normalizedMessage, new[] { "تم الاختبار" }))
                return "تم تقييم حالتك بناءً على إجاباتك. يمكنك مراجعة متخصص لفهم النتائج بشكل أعمق.";

            if (ContainsWord(normalizedMessage, new[] { "ما هي المنصة" }))
                return "المنصة مخصصة لتقديم تقييم مبدئي يساعد الأطفال على معرفة نسبة التأخر العقلي لديهم بناءً على أسئلة معيارية مصممة بدقة.";

            if (ContainsWord(normalizedMessage, new[] { "ما هدف المنصة" }))
                return "هدف المنصة هو مساعدة الأطفال وأولياء الأمور على تحديد الاحتياجات الخاصة والتوجيه للحصول على المساعدة المناسبة من المتخصصين.";

            if (ContainsWord(normalizedMessage, new[] { "كيف أحسن من مستوى ابني" }))
                return "لتحسين مستوى ابنك، يمكنك التركيز على تنمية مهاراته من خلال اللعب التعليمي، القراءة معه يوميًا، وتعزيز تواصله الاجتماعي. كما يُفضل التواصل مع متخصص لتحديد الاحتياجات المحددة له.";

            if (ContainsWord(normalizedMessage, new[] { "توجيهات للآباء" }))
                return "التوجيهات تشمل: توفير بيئة آمنة ومحفزة، التحدث مع الطفل بانتظام لتحسين لغته، تشجيعه على التعلم من خلال الأنشطة الممتعة، والمشاركة في جلسات مع متخصص إذا لزم الأمر.";

            if (ContainsWord(normalizedMessage, new[] { "كيف أتعامل معه وقت الغضب" }))
                return "وقت الغضب، حاول أن تبقى هادئًا وتجنب الصراخ. اترك له مساحة للتعبير عن مشاعره ثم تحدث معه بهدوء لمعرفة السبب. يمكنك استخدام تقنيات مثل التنفس العميق أو تشتيت الانتباه بممارسة نشاط يحبه.";

            // الرد الافتراضي
            return "عذرًا، لم أفهم رسالتك. هل يمكنك إعادة صياغتها؟";
        }

        private bool ContainsWord(string input, string[] words)
        {
            return words.Any(word => input.Contains(word));
        }
    }

    public class UserMessage
    {
        public string Message { get; set; }
    }
}
