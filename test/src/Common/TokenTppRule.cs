using System.Collections.Generic;
using Io.Token.Proto.Gateway.Testing;
using Tokenio.Proto.Common.AliasProtos;
using Tokenio.Proto.Common.MemberProtos;
using Tokenio.Utils;
using Xunit;

namespace Tokenio.BankSample.Common
{
    public class TokenTppRule : TokenRule
    {
        private readonly Tpp.TokenClient tokenClient;

        public TokenTppRule()
        {
            tokenClient = Tpp.TokenClient.NewBuilder()
                .HostName(
                    envConfig.GetGateway()
                        .Host)
                .Port(
                    envConfig.GetGateway()
                        .Port)
                .Timeout(timeoutMs)
                .DeveloperKey(envConfig.GetDevKey())
                .Build();
        }

        public Tpp.Member Member()
        {
            return Member(Sample.alias());
        }

        public Tpp.Member Member(Alias alias)
        {
            return Member(CreateMemberType.Business, alias);
        }

        public Tpp.Member Member(CreateMemberType type, Alias alias = null)
        {
            var request = new CreateTestMemberRequest
            {
                MemberType = type,
                Nonce = Util.Nonce(),
                PartnerId = ""
            };
            var memberId = testingGateway.CreateTestMember(request)
                .MemberId;
            var member = Token()
                .SetUpMemberBlocking(memberId, alias);
            if (alias != null && alias.Value.Contains("+noverify@"))
            {
                ISet<string> list = new HashSet<string>();
                foreach (var i in member.GetAliasesBlocking())
                    list.Add(i.Value);
                Assert.Contains(alias.Value, list);
            }
            return member;
        }

        public Tpp.TokenClient Token()
        {
            return tokenClient;
        }
    }
}
