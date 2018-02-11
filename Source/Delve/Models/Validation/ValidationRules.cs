using System.Collections.Generic;
using System.Linq;

using Delve.Models.Expression;

namespace Delve.Models.Validation
{
    internal class ValidationRules
    {
        private readonly IList<IValidationRule> _rules = new List<IValidationRule>();

        public ValidationRules(IValidationRule rule)
        {
            AddRule(rule);
        }

        public void AddRule(IValidationRule rule)
        {
            var type = rule.ValidationType; 
            if (_rules.Select(x => x.ValidationType).Contains(type))
            {
                throw new InvalidValidationBuilderException("Cannot assign two " +
                "ValidationRules of same type (e.g. filter and filter) on same key.");
            }
            
            _rules.Add(rule);
        }

        private void CheckForValidationType(ValidationType type, string key)
        {
            if (!_rules.Select(x => x.ValidationType).Contains(type))
            {
                throw new InvalidQueryException($"{type} has not been registered under key: {key}");
            }
        }

        public string ValidateExpression(ValidationType type, INonValueExpression expression)
        {
            CheckForValidationType(type, expression.Key);
            return _rules.SingleOrDefault(x => x.ValidationType == type).ValidateExpression(expression);
        }

        public string ValidateValueExpression(ValidationType type, IValueExpression expression)
        {
            CheckForValidationType(type, expression.Key);
            return _rules.SingleOrDefault(x => x.ValidationType == type).ValidateValueExpression(expression);
        }
    }
}
