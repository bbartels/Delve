using System;
using System.Collections.Generic;
using System.Linq;

using Delve.Models.Expressions;

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

        public IValidationRule ValidateExpression(ValidationType type, IExpression exp)
        {
            CheckForValidationType(type, exp.Key);
            return _rules.SingleOrDefault(x => x.ValidationType == type).ValidateExpression(exp);
        }

        public Type GetResultType(ValidationType type, string key)
        {
            CheckForValidationType(type, key);
            return _rules.SingleOrDefault(x => x.ValidationType == type).ResultType;
        }

        private void CheckForValidationType(ValidationType type, string key)
        {
            if (!_rules.Select(x => x.ValidationType).Contains(type))
            {
                throw new InvalidQueryException($"{type} has not been registered under key: {key}");
            }
        }
    }
}
