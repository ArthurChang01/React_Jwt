import "es6-promise";
import "fetch";
import * as toastr from 'toastr';
import {LoginRequestAction} from './LoginRequestAction';
import {LoginSuccessAction} from './LoginSuccessAction';
import {LoginFailAction} from './LoginFailAction';

export function LoginAsyncAction(username, password) {
    return dispatch => {
        if (!username || !password) {
            toastr.error("username or password can't be empty!");
            return;
        }

        dispatch(LoginRequestAction());

        let fetch_Parm = {
            headers: { "Content-Type": 'application/json'},
            method: 'GET',
            mode: 'cors'
        };

        fetch(`http://localhost:12026/api/Auth?username=${username}&password=${password}`, fetch_Parm)
            .then(resp => {
                if (resp.ok){
                    let data = resp.json();
                    console.log(data);
                    dispatch(LoginSuccessAction(data["access_token"]));
                }
                else
                    dispatch(LoginFailAction('username or password is not matched'));
            })
            .catch(err => {
                dispatch(LoginFailAction(err));
            });
    }
}