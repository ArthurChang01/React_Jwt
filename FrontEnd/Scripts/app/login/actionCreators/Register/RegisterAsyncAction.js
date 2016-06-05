import "es6-promise";
import "fetch";
import * as toastr from 'toastr';
import {LoginAsyncAction} from '../Login/LoginAsyncAction';
import {RegisterRequestAction} from './RegisterRequestAction';
import {RegisterSuccessAction} from './RegisterSuccessAction';
import {RegisterFailAction} from './RegisterFailAction';

export function RegisterAsyncAction(username, email, password, confirmed_password) {
    return dispatch => {
        if (password !== confirmed_password) {
            dispatch(RegisterFailAction("password is not matched with confirmed_password"));
            return;
        }

        if (!username || !email || !password || !confirmed_password) {
            toastr.error("username, email, password and confirmed_password can't be empty!");
            return;
        }

        dispatch(RegisterRequestAction());

        let fetch_Parm = {
            headers: { "Content-Type": 'application/json' },
            method: 'POST',
            mode: 'cors',
            body: JSON.stringify({ Username: username, "Password": password, "Email": email })
        };

        fetch('http://localhost:12026/create', fetch_Parm)
            .then(resp => {
                if (resp.ok) {
                    let data = resp.json();
                    dispatch(RegisterSuccessAction(data));
                    dispatch(LoginAsyncAction(username, password));
                }
                else
                    dispatch(RegisterFailAction(resp.statusText));
            })
            .catch(err => {
                dispatch(RegisterFailAction(err));
            });
    }
}