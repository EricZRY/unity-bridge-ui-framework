﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BridgeUI.Binding
{
    public class ViewModelBase: Dictionary<string, IBindableProperty>
    {
        private List<IBindingContext> _contexts = new List<IBindingContext>();
        protected List<IBindingContext> Contexts { get { return _contexts; } }

        public ViewModelBase()
        {
            RegistPropertys();
        }

        private void RegistPropertys()
        {
            var fields = this.GetType().GetFields(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field != null && typeof(IBindableProperty).IsAssignableFrom(field.FieldType))
                {
                    var customAttributes = field.GetCustomAttributes(false);
                    var bindingProp = customAttributes.Where(x => x.GetType() == typeof(BindingAttribute)).FirstOrDefault();
                    string bindingKey = field.Name;
                    if (bindingProp != null)
                    {
                        var customKey = (bindingProp as BindingAttribute).keyword;
                        if(!string.IsNullOrEmpty(customKey)){
                            bindingKey = customKey;
                        }
                    }
                    var value = field.GetValue(this) as IBindableProperty;
                    if (value == null)
                    {
                        value = System.Activator.CreateInstance(field.FieldType) as IBindableProperty;
                        field.SetValue(this, value);
                    }

                    this[bindingKey] = value;
                }
            }
        }

        public virtual BindableProperty<T> GetBindableProperty<T>(string name)
        {
            if (!ContainsKey(name) || !(this[name] is BindableProperty<T>))
            {
                this[name] = new BindableProperty<T>();
            }
            return this[name] as BindableProperty<T>;
        }

        public virtual IBindableProperty GetBindableProperty(string name, System.Type type)
        {
            var fullType = typeof(BindableProperty<>).MakeGenericType(type);

            if (!ContainsKey(name) || this[name].GetType() != fullType)
            {
                this[name] = System.Activator.CreateInstance(fullType) as IBindableProperty;
            }
            return this[name] as IBindableProperty;
        }

        protected virtual T GetValue<T>(string key)
        {
            return GetBindableProperty<T>(key).Value;
        }
        protected virtual void SetValue<T>(string key,T value)
        {
            GetBindableProperty<T>(key).Value = value;
        }
        public virtual void OnBinding(IBindingContext context) { this._contexts.Add(context); }
        public virtual void OnUnBinding(IBindingContext context) { this._contexts.Remove(context); }
    }

}
